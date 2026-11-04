namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class RecurringExpenseResponseModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public int DayOfMonth { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastGeneratedOn { get; set; }
    }
}
