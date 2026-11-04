namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public class Income
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Amount { get; set; }
        public DateTime TrxDate { get; set; }
        public int UserId { get; set; }
        public int? RecurringIncomeId { get; set; }

        public Category Category { get; set; }
        public RecurringIncome RecurringIncome { get; set; }
    }
}
