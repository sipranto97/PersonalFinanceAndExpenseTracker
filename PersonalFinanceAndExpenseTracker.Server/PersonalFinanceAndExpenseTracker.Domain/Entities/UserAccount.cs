namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? GoogleId { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // ISO 4217 code. Display-only: stored amounts are never converted.
        public string Currency { get; set; } = "USD";

        // True for accounts whose currency was auto-picked (e.g. first Google sign-in)
        // and hasn't been confirmed by the user yet.
        public bool NeedsCurrencySetup { get; set; }

        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Income>? Incomes { get; set; }
    }
}
