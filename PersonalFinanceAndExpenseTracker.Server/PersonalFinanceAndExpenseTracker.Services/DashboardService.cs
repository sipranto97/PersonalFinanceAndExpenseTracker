using Microsoft.EntityFrameworkCore;
using PersonalFinanceAndExpenseTracker.Domain;
using PersonalFinanceAndExpenseTracker.Domain.Entities;
using PersonalFinanceAndExpenseTracker.Services.Models;

namespace PersonalFinanceAndExpenseTracker.Services
{
    public class DashboardService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserService _userService;

        public DashboardService(ExpenseTrackerDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<BalanceResponseModel> GetBalance(DateTime? startDate, DateTime? endDate)
        {
            var user = await _userService.GetUserInfo();

            return await GetBalance(user.Id, startDate, endDate);
        }

        public async Task<BalanceResponseModel> GetBalance(int userId, DateTime? startDate, DateTime? endDate)
        {
            var incomeQuery = _context.Incomes.Where(x => x.UserId == userId);
            var expenseQuery = _context.Expenses.Where(x => x.UserId == userId);

            if (startDate.HasValue)
            {
                incomeQuery = incomeQuery.Where(x => x.TrxDate.Date >= startDate.Value.Date);
                expenseQuery = expenseQuery.Where(x => x.TrxDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                incomeQuery = incomeQuery.Where(x => x.TrxDate.Date <= endDate.Value.Date);
                expenseQuery = expenseQuery.Where(x => x.TrxDate.Date <= endDate.Value.Date);
            }

            var totalIncome = await incomeQuery.SumAsync(x => x.Amount);
            var totalExpense = await expenseQuery.SumAsync(x => x.Amount);

            return new BalanceResponseModel
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense
            };
        }

        public async Task<List<OverviewChartPointModel>> GetOverviewChart(DateTime startDate, DateTime endDate)
        {
            var user = await _userService.GetUserInfo();

            return await GetOverviewChart(user.Id, startDate, endDate);
        }

        public async Task<List<OverviewChartPointModel>> GetOverviewChart(int userId, DateTime startDate, DateTime endDate)
        {
            var incomes = await _context.Incomes
                .Where(x => x.UserId == userId && x.TrxDate.Date >= startDate.Date && x.TrxDate.Date <= endDate.Date)
                .ToListAsync();

            var expenses = await _context.Expenses
                .Where(x => x.UserId == userId && x.TrxDate.Date >= startDate.Date && x.TrxDate.Date <= endDate.Date)
                .ToListAsync();

            var points = new List<OverviewChartPointModel>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var income = incomes.Where(x => x.TrxDate.Date == date).Sum(x => x.Amount);
                var expense = expenses.Where(x => x.TrxDate.Date == date).Sum(x => x.Amount);

                points.Add(new OverviewChartPointModel
                {
                    Date = date,
                    Income = income,
                    Expense = expense,
                    Balance = income - expense
                });
            }

            return points;
        }

        public async Task<List<ChartSeriesPointModel>> GetExpenseChart(DateTime startDate, DateTime endDate)
        {
            var user = await _userService.GetUserInfo();

            return await GetExpenseChart(user.Id, startDate, endDate);
        }

        public async Task<List<ChartSeriesPointModel>> GetExpenseChart(int userId, DateTime startDate, DateTime endDate)
        {
            var expenses = await _context.Expenses
                .Include(x => x.Category)
                .Where(x => x.UserId == userId && x.TrxDate.Date >= startDate.Date && x.TrxDate.Date <= endDate.Date)
                .ToListAsync();

            return BuildSeries(expenses, x => x.TrxDate, x => x.Amount, x => x.Category.Name, startDate, endDate);
        }

        public async Task<List<ChartSeriesPointModel>> GetIncomeChart(DateTime startDate, DateTime endDate)
        {
            var user = await _userService.GetUserInfo();

            return await GetIncomeChart(user.Id, startDate, endDate);
        }

        public async Task<List<ChartSeriesPointModel>> GetIncomeChart(int userId, DateTime startDate, DateTime endDate)
        {
            var incomes = await _context.Incomes
                .Include(x => x.Category)
                .Where(x => x.UserId == userId && x.TrxDate.Date >= startDate.Date && x.TrxDate.Date <= endDate.Date)
                .ToListAsync();

            return BuildSeries(incomes, x => x.TrxDate, x => x.Amount, x => x.Category.Name, startDate, endDate);
        }

        private static List<ChartSeriesPointModel> BuildSeries<T>(
            List<T> transactions,
            Func<T, DateTime> getDate,
            Func<T, int> getAmount,
            Func<T, string> getCategory,
            DateTime startDate,
            DateTime endDate)
        {
            var points = new List<ChartSeriesPointModel>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dayTransactions = transactions.Where(x => getDate(x).Date == date).ToList();

                points.Add(new ChartSeriesPointModel
                {
                    Date = date,
                    Amount = dayTransactions.Sum(getAmount),
                    CategoryBreakdown = dayTransactions
                        .GroupBy(getCategory)
                        .Select(g => new CategoryAmountModel { Category = g.Key, Amount = g.Sum(getAmount) })
                        .ToList()
                });
            }

            return points;
        }
    }
}
