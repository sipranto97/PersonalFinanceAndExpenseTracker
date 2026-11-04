using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class RecurringIncomeService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;

        public RecurringIncomeService(ExpenseTrackerDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task CreateRecurringIncome(CreateRecurringIncomeRequestModel model)
        {
            if (model.DayOfMonth < 1 || model.DayOfMonth > 31)
            {
                throw new ArgumentException("Day of month must be between 1 and 31.");
            }

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

            var recurringIncome = new RecurringIncome
            {
                CategoryId = categoryId,
                Amount = model.Amount,
                DayOfMonth = model.DayOfMonth,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _context.RecurringIncomes.AddAsync(recurringIncome);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RecurringIncomeResponseModel>> GetRecurringIncomes()
        {
            var user = await _userService.GetUserInfo();

            var recurringIncomes = await _context.RecurringIncomes.Include(x => x.Category)
                                                .Where(x => x.UserId == user.Id)
                                                .Select(x => new RecurringIncomeResponseModel
                                                {
                                                    Id = x.Id,
                                                    Category = x.Category.Name,
                                                    CategoryId = x.CategoryId,
                                                    Amount = x.Amount,
                                                    DayOfMonth = x.DayOfMonth,
                                                    IsActive = x.IsActive,
                                                    LastGeneratedOn = x.LastGeneratedOn,
                                                })
                                                .ToListAsync();

            return recurringIncomes;
        }

        public async Task UpdateRecurringIncome(int id, UpdateRecurringIncomeRequestModel model)
        {
            if (model.DayOfMonth < 1 || model.DayOfMonth > 31)
            {
                throw new ArgumentException("Day of month must be between 1 and 31.");
            }

            var user = await _userService.GetUserInfo();

            var recurringIncome = await _context.RecurringIncomes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (recurringIncome == null)
            {
                throw new KeyNotFoundException("Recurring income not found");
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

            recurringIncome.CategoryId = categoryId;
            recurringIncome.Amount = model.Amount;
            recurringIncome.DayOfMonth = model.DayOfMonth;

            await _context.SaveChangesAsync();
        }

        public async Task CancelRecurringIncome(int id)
        {
            var user = await _userService.GetUserInfo();

            var recurringIncome = await _context.RecurringIncomes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (recurringIncome == null)
            {
                throw new KeyNotFoundException("Recurring income not found");
            }

            recurringIncome.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}
