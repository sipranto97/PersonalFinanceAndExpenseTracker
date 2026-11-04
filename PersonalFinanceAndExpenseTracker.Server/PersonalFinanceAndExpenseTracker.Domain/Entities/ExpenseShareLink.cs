namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public class ExpenseShareLink
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

        public UserAccount User { get; set; }
    }
}
