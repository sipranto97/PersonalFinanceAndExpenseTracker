namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class CreateExpenseRequestModel
    {
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public DateTime TrxDate { get; set; }
    }
}
