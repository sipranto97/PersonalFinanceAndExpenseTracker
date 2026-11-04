namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class UpdateSavingsGoalRequestModel
    {
        public string Name { get; set; }
        public int TargetAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public int? MonthlyContribution { get; set; }
    }
}
