namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class SharedIncomeHistoryResponseModel
    {
        public string OwnerName { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<SharedIncomeItemModel> Incomes { get; set; }
    }
}
