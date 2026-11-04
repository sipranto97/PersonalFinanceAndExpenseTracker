using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class ExpenseService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;

        public ExpenseService(ExpenseTrackerDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task CreateExpense(CreateExpenseRequestModel model)
        {
            try
            {
                var user = await _userService.GetUserInfo();

                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }

                var categoryId = model.CategoryId;

                if (categoryId == -1)
                {
                    var category = new Category
                    {
                        Name = model.Category,
                        IsIncome = false,
                        UserId = user.Id
                    };

                    await _context.Categories.AddAsync(category);
                    await _context.SaveChangesAsync();

                    categoryId = category.Id;
                }

                var expense = new Expense
                {
                    CategoryId = categoryId,
                    Amount = model.Amount,
                    TrxDate = model.TrxDate,
                    UserId = user.Id
                };

                await _context.Expenses.AddAsync(expense);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured While Creating Expense");
            }
        }

        public async Task<List<ExpenseResponseModel>> GetExpenses()
        {
            var user = await _userService.GetUserInfo();

            var expenses = await _context.Expenses.Include(x => x.Category)
                                                .Where(x => x.UserId == user.Id)
                                                .Select(x => new ExpenseResponseModel
                                                {
                                                    Id = x.Id,
                                                    Category = x.Category.Name,
                                                    CategoryId = x.CategoryId,
                                                    Amount = x.Amount,
                                                    TrxDate = x.TrxDate,
                                                })
                                                .ToListAsync();


            return expenses;
        }
        public async Task<List<CategoryResponseModel>> GetCategories()
        {
            var user = await _userService.GetUserInfo();

            var categories = await _context.Categories.Where(x => x.UserId == user.Id && x.IsIncome == false)
                                                .Select(x => new CategoryResponseModel
                                                {
                                                    Id = x.Id,
                                                    Name = x.Name,
                                                })
                                                .ToListAsync();


            return categories;
        }

        public async Task UpdateExpense(int id, UpdateExpenseRequestModel model)
        {
            var user = await _userService.GetUserInfo();

            var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (expense == null)
            {
                throw new KeyNotFoundException("Expense not found");
            }

            var categoryId = model.CategoryId;

            if (categoryId == -1)
            {
                var category = new Category
                {
                    Name = model.Category,
                    IsIncome = false,
                    UserId = user.Id
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                categoryId = category.Id;
            }

            expense.CategoryId = categoryId;
            expense.Amount = model.Amount;
            expense.TrxDate = model.TrxDate;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpense(int id)
        {
            var user = await _userService.GetUserInfo();

            var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if(expense == null)
            {
                throw new KeyNotFoundException("Expense not found");
            }

            var contribution = await _context.SavingsGoalContributions
                .Include(x => x.SavingsGoal)
                .FirstOrDefaultAsync(x => x.ExpenseId == id);

            if (contribution != null)
            {
                if (contribution.SavingsGoal.Status == SavingsGoalStatus.Settled)
                {
                    throw new InvalidOperationException("This expense funded a settled savings goal and cannot be deleted.");
                }

                _context.SavingsGoalContributions.Remove(contribution);

                var remainingSaved = await _context.SavingsGoalContributions
                    .Where(x => x.SavingsGoalId == contribution.SavingsGoalId && x.Id != contribution.Id)
                    .SumAsync(x => x.Amount);

                if (contribution.SavingsGoal.Status == SavingsGoalStatus.Completed
                    && remainingSaved < contribution.SavingsGoal.TargetAmount)
                {
                    contribution.SavingsGoal.Status = SavingsGoalStatus.Active;
                }
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

        }

    }
}
