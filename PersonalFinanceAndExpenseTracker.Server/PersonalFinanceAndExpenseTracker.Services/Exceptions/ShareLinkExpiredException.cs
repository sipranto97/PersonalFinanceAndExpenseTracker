namespace PersonalFinanceAndExpenseTracker.Services.Exceptions
{
    public class ShareLinkExpiredException : Exception
    {
        public ShareLinkExpiredException(string message) : base(message)
        {
        }
    }
}
