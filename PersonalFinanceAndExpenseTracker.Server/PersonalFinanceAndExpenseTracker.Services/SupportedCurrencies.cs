namespace PersonalFinanceAndExpenseTracker.Services
{
    /// <summary>
    /// ISO 4217 currencies a user can pick. Currency is display-only, so this list
    /// only controls which codes are accepted; it must match the list in the Angular client.
    /// </summary>
    public static class SupportedCurrencies
    {
        public const string Default = "USD";

        private static readonly HashSet<string> Codes = new(StringComparer.OrdinalIgnoreCase)
        {
            "USD", "EUR", "GBP", "BDT", "INR", "PKR", "LKR", "NPR", "JPY", "CNY",
            "KRW", "SGD", "MYR", "IDR", "THB", "PHP", "VND", "AED", "SAR", "QAR",
            "KWD", "TRY", "AUD", "NZD", "CAD", "CHF", "SEK", "NOK", "DKK", "BRL",
            "MXN", "ZAR", "NGN", "EGP", "KES"
        };

        public static bool IsSupported(string? code)
            => !string.IsNullOrWhiteSpace(code) && Codes.Contains(code.Trim());

        public static string Normalize(string? code)
            => IsSupported(code) ? code!.Trim().ToUpperInvariant() : Default;
    }
}
