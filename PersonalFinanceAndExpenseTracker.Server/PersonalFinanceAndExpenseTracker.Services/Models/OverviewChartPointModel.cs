namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class OverviewChartPointModel
    {
        public DateTime Date { get; set; }
        public int Income { get; set; }
        public int Expense { get; set; }
        public int Balance { get; set; }
    }
}
