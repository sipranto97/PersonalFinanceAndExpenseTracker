namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    public enum SavingsGoalStatus
    {
        Active,
        Completed,
        Settled
    }

    public enum SavingsGoalSettlementAction
    {
        AddedToBalance,
        Expensed
    }

    public class SavingsGoal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TargetAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public int? MonthlyContribution { get; set; }
        public SavingsGoalStatus Status { get; set; }
        public SavingsGoalSettlementAction? SettlementAction { get; set; }
        public int? SettlementIncomeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

        public UserAccount User { get; set; }
        public Income SettlementIncome { get; set; }
        public ICollection<SavingsGoalContribution> Contributions { get; set; }
    }
}
