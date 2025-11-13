namespace AuthNet.UI.Models.DTO
{
    public class DashboardViewModel
    {
        public DailyProfitDto DailySales { get; set; }
        public MonthlyProfitDto MonthlySales { get; set; }
        public YearlyProfitDto YearlySales { get; set; }
        public DailySalesDto DailySale { get; set; }
        public MonthlySalesDto MonthlySale { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public List<TaskItemDto> Tasks { get; set; } = new();
    }
}
