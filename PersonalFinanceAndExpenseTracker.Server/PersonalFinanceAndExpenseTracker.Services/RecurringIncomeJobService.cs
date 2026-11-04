using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class RecurringIncomeJobService
    {
        private readonly ExpenseTrackerDbContext _context;

        public RecurringIncomeJobService(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task GenerateDueIncomes()
        {
            var today = DateTime.Today;

            var activeRecurringIncomes = await _context.RecurringIncomes
                .Where(x => x.IsActive)
                .ToListAsync();

            foreach (var recurringIncome in activeRecurringIncomes)
            {
                var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                var targetDay = Math.Min(recurringIncome.DayOfMonth, daysInMonth);

                var alreadyGeneratedThisMonth = recurringIncome.LastGeneratedOn.HasValue &&
                    recurringIncome.LastGeneratedOn.Value.Year == today.Year &&
                    recurringIncome.LastGeneratedOn.Value.Month == today.Month;

                if (alreadyGeneratedThisMonth || today.Day < targetDay)
                {
                    continue;
                }

                var income = new Income
                {
                    CategoryId = recurringIncome.CategoryId,
                    Amount = recurringIncome.Amount,
                    TrxDate = new DateTime(today.Year, today.Month, targetDay),
                    UserId = recurringIncome.UserId,
                    RecurringIncomeId = recurringIncome.Id
                };

                await _context.Incomes.AddAsync(income);

                recurringIncome.LastGeneratedOn = today;
            }

            await _context.SaveChangesAsync();
        }
    }
}
