using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class RecurringExpenseService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;

        public RecurringExpenseService(ExpenseTrackerDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task CreateRecurringExpense(CreateRecurringExpenseRequestModel model)
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
                    IsIncome = false,
                    UserId = user.Id
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                categoryId = category.Id;
            }

            var recurringExpense = new RecurringExpense
            {
                CategoryId = categoryId,
                Amount = model.Amount,
                DayOfMonth = model.DayOfMonth,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _context.RecurringExpenses.AddAsync(recurringExpense);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RecurringExpenseResponseModel>> GetRecurringExpenses()
        {
            var user = await _userService.GetUserInfo();

            var recurringExpenses = await _context.RecurringExpenses.Include(x => x.Category)
                                                .Where(x => x.UserId == user.Id)
                                                .Select(x => new RecurringExpenseResponseModel
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

            return recurringExpenses;
        }

        public async Task UpdateRecurringExpense(int id, UpdateRecurringExpenseRequestModel model)
        {
            if (model.DayOfMonth < 1 || model.DayOfMonth > 31)
            {
                throw new ArgumentException("Day of month must be between 1 and 31.");
            }

            var user = await _userService.GetUserInfo();

            var recurringExpense = await _context.RecurringExpenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (recurringExpense == null)
            {
                throw new KeyNotFoundException("Recurring expense not found");
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

            recurringExpense.CategoryId = categoryId;
            recurringExpense.Amount = model.Amount;
            recurringExpense.DayOfMonth = model.DayOfMonth;

            await _context.SaveChangesAsync();
        }

        public async Task CancelRecurringExpense(int id)
        {
            var user = await _userService.GetUserInfo();

            var recurringExpense = await _context.RecurringExpenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

            if (recurringExpense == null)
            {
                throw new KeyNotFoundException("Recurring expense not found");
            }

            recurringExpense.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}
