namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public class RecurringIncome
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Amount { get; set; }
        public int DayOfMonth { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastGeneratedOn { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

        public UserAccount User { get; set; }
        public Category Category { get; set; }
        public ICollection<Income> GeneratedIncomes { get; set; }
    }
}
