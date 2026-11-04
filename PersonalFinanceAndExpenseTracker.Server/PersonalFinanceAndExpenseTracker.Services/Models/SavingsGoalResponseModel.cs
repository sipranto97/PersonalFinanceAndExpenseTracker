namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class SavingsGoalResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TargetAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public int? MonthlyContribution { get; set; }
        public int AmountSaved { get; set; }
        public int RemainingAmount { get; set; }
        public double PercentageProgress { get; set; }
        public string Status { get; set; }
        public string SettlementAction { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaceStatus { get; set; }
        public int? PaceDifference { get; set; }
    }
}
