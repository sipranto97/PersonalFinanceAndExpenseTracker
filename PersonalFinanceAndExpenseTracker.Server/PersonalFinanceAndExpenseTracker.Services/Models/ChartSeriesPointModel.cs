namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class ChartSeriesPointModel
    {
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public List<CategoryAmountModel> CategoryBreakdown { get; set; }
    }
}
