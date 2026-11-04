namespace PersonalFinanceAndExpenseTracker.Services.Models
{
    public class SettleGoalResponseModel
    {
        public int GoalId { get; set; }
        public string Status { get; set; }
        public string SettlementAction { get; set; }
    }
}
