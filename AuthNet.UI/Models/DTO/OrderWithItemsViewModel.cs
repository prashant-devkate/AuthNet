namespace AuthNet.UI.Models.DTO
{
    public class OrderWithItemsViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new();

    }
}
