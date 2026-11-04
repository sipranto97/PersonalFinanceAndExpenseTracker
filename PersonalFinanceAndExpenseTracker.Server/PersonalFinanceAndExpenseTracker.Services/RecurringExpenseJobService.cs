using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class RecurringExpenseJobService
    {
        private readonly ExpenseTrackerDbContext _context;

        public RecurringExpenseJobService(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task GenerateDueExpenses()
        {
            var today = DateTime.Today;

            var activeRecurringExpenses = await _context.RecurringExpenses
                .Where(x => x.IsActive)
                .ToListAsync();

            foreach (var recurringExpense in activeRecurringExpenses)
            {
                var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                var targetDay = Math.Min(recurringExpense.DayOfMonth, daysInMonth);

                var alreadyGeneratedThisMonth = recurringExpense.LastGeneratedOn.HasValue &&
                    recurringExpense.LastGeneratedOn.Value.Year == today.Year &&
                    recurringExpense.LastGeneratedOn.Value.Month == today.Month;

                if (alreadyGeneratedThisMonth || today.Day < targetDay)
                {
                    continue;
                }

                var expense = new Expense
                {
                    CategoryId = recurringExpense.CategoryId,
                    Amount = recurringExpense.Amount,
                    TrxDate = new DateTime(today.Year, today.Month, targetDay),
                    UserId = recurringExpense.UserId,
                    RecurringExpenseId = recurringExpense.Id
                };

                await _context.Expenses.AddAsync(expense);

                recurringExpense.LastGeneratedOn = today;
            }

            await _context.SaveChangesAsync();
        }
    }
}
