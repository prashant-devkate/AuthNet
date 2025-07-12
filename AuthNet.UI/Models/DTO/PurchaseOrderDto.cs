namespace AuthNet.UI.Models.DTO
{
    public class PurchaseOrderDto
    {
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Username { get; set; }
        public string Suppliername { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
    public class PurchaseOrderItemDto
    {
        public int PurchaseOrderItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
