using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class SavingsGoalService
    {
        private const int MaxActiveGoalsOnDashboard = 3;

        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;

        public SavingsGoalService(ExpenseTrackerDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<SavingsGoalResponseModel> CreateGoal(CreateSavingsGoalRequestModel model)
        {
            if (model.TargetAmount <= 0)
            {
                throw new ArgumentException("Target amount must be greater than zero.");
            }

            if (model.TargetDate.Date < DateTime.UtcNow.Date)
            {
                throw new ArgumentException("Target date must be today or later.");
            }

            var user = await _userService.GetUserInfo();

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var goal = new SavingsGoal
            {
                Name = model.Name,
                TargetAmount = model.TargetAmount,
                TargetDate = model.TargetDate.Date,
                MonthlyContribution = model.MonthlyContribution,
                Status = SavingsGoalStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _context.SavingsGoals.AddAsync(goal);
            await _context.SaveChangesAsync();

            return MapToResponse(goal, 0);
        }

        public async Task<List<SavingsGoalResponseModel>> GetGoals()
        {
            var user = await _userService.GetUserInfo();

            var goals = await _context.SavingsGoals
                .Include(x => x.Contributions)
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            return goals
                .Select(x => MapToResponse(x, x.Contributions.Sum(c => c.Amount)))
                .ToList();
        }

        public async Task<List<SavingsGoalResponseModel>> GetActiveGoals()
        {
            var user = await _userService.GetUserInfo();

            var goals = await _context.SavingsGoals
                .Include(x => x.Contributions)
                .Where(x => x.UserId == user.Id && x.Status == SavingsGoalStatus.Active)
                .OrderBy(x => x.TargetDate)
                .Take(MaxActiveGoalsOnDashboard)
                .ToListAsync();

            return goals
                .Select(x => MapToResponse(x, x.Contributions.Sum(c => c.Amount)))
                .ToList();
        }

        public async Task<ContributionResponseModel> AddContribution(int id, AddContributionRequestModel model)
        {
            if (model.Amount <= 0)
            {
                throw new ArgumentException("Contribution amount must be greater than zero.");
            }

            var user = await _userService.GetUserInfo();

            var goal = await _context.SavingsGoals
                .Include(x => x.Contributions)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (goal == null)
            {
                throw new KeyNotFoundException("Savings goal not found");
            }

            if (goal.Status == SavingsGoalStatus.Settled)
            {
                throw new InvalidOperationException("This goal has already been settled.");
            }

            var totalIncome = await _context.Incomes.Where(x => x.UserId == user.Id).SumAsync(x => x.Amount);
            var totalExpense = await _context.Expenses.Where(x => x.UserId == user.Id).SumAsync(x => x.Amount);
            var currentBalance = totalIncome - totalExpense;

            if (model.Amount > currentBalance)
            {
                throw new InvalidOperationException($"Contribution exceeds your current balance of {currentBalance}.");
            }

            var categoryId = await GetOrCreateCategory(user.Id, $"Savings Goal - {goal.Name}", isIncome: false);

            var expense = new Expense
            {
                CategoryId = categoryId,
                Amount = model.Amount,
                TrxDate = DateTime.UtcNow,
                UserId = user.Id
            };

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            var amountSavedBefore = goal.Contributions.Sum(c => c.Amount);

            var contribution = new SavingsGoalContribution
            {
                SavingsGoalId = goal.Id,
                Amount = model.Amount,
                ContributedOn = DateTime.UtcNow,
                ExpenseId = expense.Id
            };

            await _context.SavingsGoalContributions.AddAsync(contribution);

            var amountSavedAfter = amountSavedBefore + model.Amount;
            var justCompleted = goal.Status == SavingsGoalStatus.Active && amountSavedAfter >= goal.TargetAmount;

            if (justCompleted)
            {
                goal.Status = SavingsGoalStatus.Completed;
            }

            await _context.SaveChangesAsync();

            return new ContributionResponseModel
            {
                GoalId = goal.Id,
                AmountSaved = amountSavedAfter,
                RemainingAmount = Math.Max(0, goal.TargetAmount - amountSavedAfter),
                PercentageProgress = CalculatePercentage(amountSavedAfter, goal.TargetAmount),
                Status = goal.Status.ToString(),
                IsJustCompleted = justCompleted
            };
        }

        public async Task<SettleGoalResponseModel> SettleGoal(int id, SettleGoalRequestModel model)
        {
            var user = await _userService.GetUserInfo();

            var goal = await _context.SavingsGoals
                .Include(x => x.Contributions)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (goal == null)
            {
                throw new KeyNotFoundException("Savings goal not found");
            }

            if (goal.Status != SavingsGoalStatus.Completed)
            {
                throw new InvalidOperationException("Only a completed goal can be settled.");
            }

            var amountSaved = goal.Contributions.Sum(c => c.Amount);

            if (model.Action == "AddToBalance")
            {
                var categoryId = await GetOrCreateCategory(user.Id, $"Savings Goal Withdrawal - {goal.Name}", isIncome: true);

                var income = new Income
                {
                    CategoryId = categoryId,
                    Amount = amountSaved,
                    TrxDate = DateTime.UtcNow,
                    UserId = user.Id
                };

                await _context.Incomes.AddAsync(income);
                await _context.SaveChangesAsync();

                goal.SettlementIncomeId = income.Id;
                goal.SettlementAction = SavingsGoalSettlementAction.AddedToBalance;
            }
            else if (model.Action == "Expense")
            {
                goal.SettlementAction = SavingsGoalSettlementAction.Expensed;
            }
            else
            {
                throw new ArgumentException("Action must be either 'AddToBalance' or 'Expense'.");
            }

            goal.Status = SavingsGoalStatus.Settled;

            await _context.SaveChangesAsync();

            return new SettleGoalResponseModel
            {
                GoalId = goal.Id,
                Status = goal.Status.ToString(),
                SettlementAction = goal.SettlementAction.ToString()
            };
        }

        public async Task UpdateGoal(int id, UpdateSavingsGoalRequestModel model)
        {
            if (model.TargetAmount <= 0)
            {
                throw new ArgumentException("Target amount must be greater than zero.");
            }

            var user = await _userService.GetUserInfo();

            var goal = await _context.SavingsGoals
                .Include(x => x.Contributions)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (goal == null)
            {
                throw new KeyNotFoundException("Savings goal not found");
            }

            if (goal.Status == SavingsGoalStatus.Settled)
            {
                throw new InvalidOperationException("A settled goal cannot be edited.");
            }

            goal.Name = model.Name;
            goal.TargetAmount = model.TargetAmount;
            goal.TargetDate = model.TargetDate.Date;
            goal.MonthlyContribution = model.MonthlyContribution;

            var amountSaved = goal.Contributions.Sum(c => c.Amount);

            if (amountSaved >= goal.TargetAmount)
            {
                goal.Status = SavingsGoalStatus.Completed;
            }
            else if (goal.Status == SavingsGoalStatus.Completed)
            {
                goal.Status = SavingsGoalStatus.Active;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteGoal(int id)
        {
            var user = await _userService.GetUserInfo();

            var goal = await _context.SavingsGoals
                .Include(x => x.Contributions)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (goal == null)
            {
                throw new KeyNotFoundException("Savings goal not found");
            }

            if (goal.Contributions.Any())
            {
                throw new InvalidOperationException("A goal with contributions cannot be deleted. Settle it instead.");
            }

            _context.SavingsGoals.Remove(goal);
            await _context.SaveChangesAsync();
        }

        private async Task<int> GetOrCreateCategory(int userId, string name, bool isIncome)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name && x.IsIncome == isIncome);

            if (category != null)
            {
                return category.Id;
            }

            category = new Category
            {
                Name = name,
                IsIncome = isIncome,
                UserId = userId
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category.Id;
        }

        private static double CalculatePercentage(int amountSaved, int targetAmount)
        {
            if (targetAmount <= 0)
            {
                return 0;
            }

            var percentage = (double)amountSaved / targetAmount * 100;
            return Math.Min(100, Math.Round(percentage, 2));
        }

        private static (string Status, int? Difference) CalculatePace(SavingsGoal goal, int amountSaved)
        {
            if (goal.Status != SavingsGoalStatus.Active || !goal.MonthlyContribution.HasValue || goal.MonthlyContribution.Value <= 0)
            {
                return (null, null);
            }

            var now = DateTime.UtcNow;
            var monthsElapsed = ((now.Year - goal.CreatedAt.Year) * 12) + (now.Month - goal.CreatedAt.Month);

            if (now.Day < goal.CreatedAt.Day)
            {
                monthsElapsed--;
            }

            monthsElapsed = Math.Max(monthsElapsed, 0);

            var expectedByNow = goal.MonthlyContribution.Value * monthsElapsed;
            var difference = amountSaved - expectedByNow;

            var status = difference > 0 ? "Ahead" : difference < 0 ? "Behind" : "OnTrack";

            return (status, difference);
        }

        private SavingsGoalResponseModel MapToResponse(SavingsGoal goal, int amountSaved)
        {
            var pace = CalculatePace(goal, amountSaved);

            return new SavingsGoalResponseModel
            {
                Id = goal.Id,
                Name = goal.Name,
                TargetAmount = goal.TargetAmount,
                TargetDate = goal.TargetDate,
                MonthlyContribution = goal.MonthlyContribution,
                AmountSaved = amountSaved,
                RemainingAmount = Math.Max(0, goal.TargetAmount - amountSaved),
                PercentageProgress = CalculatePercentage(amountSaved, goal.TargetAmount),
                Status = goal.Status.ToString(),
                SettlementAction = goal.SettlementAction?.ToString(),
                CreatedAt = goal.CreatedAt,
                PaceStatus = pace.Status,
                PaceDifference = pace.Difference
            };
        }
    }
}
