using PersonalFinanceAndExpenseTracker.Domain.Entities;

namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class SharedChartDataResponseModel
    {
        public string OwnerName { get; set; }
        public ChartType ChartType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<OverviewChartPointModel> Overview { get; set; }
        public List<ChartSeriesPointModel> Expense { get; set; }
        public List<ChartSeriesPointModel> Income { get; set; }
    }
}
