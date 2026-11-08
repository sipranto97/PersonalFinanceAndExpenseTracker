namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class UpdateProfileRequestModel
    {
        // Optional: left unchanged when empty.
        public string? FullName { get; set; }

        // Optional: left unchanged when empty. Must be a supported ISO 4217 code.
        public string? Currency { get; set; }
    }
}
