namespace AuthNet.Models.DTO
{
    public class OrderWithItemsDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
