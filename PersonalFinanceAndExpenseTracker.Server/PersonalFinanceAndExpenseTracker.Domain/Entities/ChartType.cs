using System.Text.Json.Serialization;

namespace PersonalFinanceAndExpenseTracker.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChartType
    {
        Overview,
        Expense,
        Income
    }
}
