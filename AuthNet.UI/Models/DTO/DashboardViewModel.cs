namespace AuthNet.UI.Models.DTO
{
    public class DashboardViewModel
    {
        public DailyProfitDto DailySales { get; set; }
        public MonthlyProfitDto MonthlySales { get; set; }
        public YearlyProfitDto YearlySales { get; set; }
        public List<TaskItemDto> Tasks { get; set; } = new();
    }
}
