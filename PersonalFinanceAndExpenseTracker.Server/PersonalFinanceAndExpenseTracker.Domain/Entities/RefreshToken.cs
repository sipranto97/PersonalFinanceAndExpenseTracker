namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenId { get; set; }
        public string RefreshUserToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserAccount User { get; set; }
    }

}
