namespace AuthNet.UI.Models.DTO
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalTasks { get; set; }
        public int TotalOrders { get; set; }
        public List<TaskItemDto> Tasks { get; set; } = new();
    }
}
