namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public bool HasPassword { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Currency { get; set; } = "USD";
        public bool NeedsCurrencySetup { get; set; }
    }
}
