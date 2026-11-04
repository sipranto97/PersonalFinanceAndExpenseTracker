namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class SharedExpenseHistoryResponseModel
    {
        public string OwnerName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<SharedExpenseItemModel> Expenses { get; set; }
    }
}
