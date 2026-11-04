using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class IncomeService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;

        public IncomeService(ExpenseTrackerDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task CreateIncome(CreateIncomeRequestModel model)
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
                        IsIncome = true,
                        UserId = user.Id
                    };

                    await _context.Categories.AddAsync(category);
                    await _context.SaveChangesAsync();

                    categoryId = category.Id;
                }

                var income = new Income
                {
                    CategoryId = categoryId,
                    Amount = model.Amount,
                    TrxDate = model.TrxDate,
                    UserId = user.Id
                };

                await _context.Incomes.AddAsync(income);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured While Creating Income");
            }
        }

        public async Task<List<IncomeResponseModel>> GetIncomes()
        {
            var user = await _userService.GetUserInfo();

            var incomes = await _context.Incomes.Include(x => x.Category)
                                                .Where(x => x.UserId == user.Id)
                                                .Select(x => new IncomeResponseModel
                                                {
                                                    Id = x.Id,
                                                    Category = x.Category.Name,
                                                    CategoryId = x.CategoryId,
                                                    Amount = x.Amount,
                                                    TrxDate = x.TrxDate,
                                                })
                                                .ToListAsync();
                                                

            return incomes;
        }
        public async Task<List<CategoryResponseModel>> GetCategories()
        {
            var user = await _userService.GetUserInfo();

            var categories = await _context.Categories.Where(x => x.UserId == user.Id && x.IsIncome == true)
                                                .Select(x => new CategoryResponseModel
                                                {
                                                    Id = x.Id,
                                                    Name = x.Name,
                                                })
                                                .ToListAsync();


            return categories;
        }

        public async Task UpdateIncome(int id, UpdateIncomeRequestModel model)
        {
            var user = await _userService.GetUserInfo();

            var income = await _context.Incomes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (income == null)
            {
                throw new KeyNotFoundException("Income not found");
            }

            var categoryId = model.CategoryId;

            if (categoryId == -1)
            {
                var category = new Category
                {
                    Name = model.Category,
                    IsIncome = true,
                    UserId = user.Id
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                categoryId = category.Id;
            }

            income.CategoryId = categoryId;
            income.Amount = model.Amount;
            income.TrxDate = model.TrxDate;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteIncome(int id)
        {
            var user = await _userService.GetUserInfo();

            var income = await _context.Incomes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if(income == null)
            {
                throw new KeyNotFoundException("Income not found");
            }

            var isGoalSettlement = await _context.SavingsGoals.AnyAsync(x => x.SettlementIncomeId == id);

            if (isGoalSettlement)
            {
                throw new InvalidOperationException("This income is the settlement for a savings goal and cannot be deleted.");
            }

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

        }

    }
}
