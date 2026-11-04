namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class CreateExpenseShareLinkRequestModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
