namespace AuthNet.UI.Models.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Username { get; set; }
        public string Customername { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
