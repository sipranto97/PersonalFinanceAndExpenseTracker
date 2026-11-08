namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }

        // Optional hint (ISO 4217) guessed by the browser from its locale.
        // Only used when this sign-in creates a new account.
        public string? Currency { get; set; }
    }
}
