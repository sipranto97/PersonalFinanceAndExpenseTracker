namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class ContributionResponseModel
    {
        public int GoalId { get; set; }
        public int AmountSaved { get; set; }
        public int RemainingAmount { get; set; }
        public double PercentageProgress { get; set; }
        public string Status { get; set; }
        public bool IsJustCompleted { get; set; }
    }
}
