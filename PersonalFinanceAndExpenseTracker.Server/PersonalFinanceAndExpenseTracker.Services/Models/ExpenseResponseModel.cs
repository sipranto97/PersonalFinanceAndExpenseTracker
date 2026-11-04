namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class ExpenseResponseModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public DateTime TrxDate { get; set; }
    }
}
