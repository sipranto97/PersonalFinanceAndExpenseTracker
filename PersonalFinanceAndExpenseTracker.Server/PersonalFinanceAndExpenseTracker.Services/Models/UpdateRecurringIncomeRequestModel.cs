namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class UpdateRecurringIncomeRequestModel
    {
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public int DayOfMonth { get; set; }
    }
}
