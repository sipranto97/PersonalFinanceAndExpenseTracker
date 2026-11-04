namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Amount { get; set; }
        public DateTime TrxDate { get; set; }
        public int UserId { get; set; }
        public int? RecurringExpenseId { get; set; }

        public Category Category { get; set; }
        public RecurringExpense RecurringExpense { get; set; }
    }
}
