namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public class SavingsGoalContribution
    {
        public int Id { get; set; }
        public int SavingsGoalId { get; set; }
        public int Amount { get; set; }
        public DateTime ContributedOn { get; set; }
        public int ExpenseId { get; set; }

        public SavingsGoal SavingsGoal { get; set; }
        public Expense Expense { get; set; }
    }
}
