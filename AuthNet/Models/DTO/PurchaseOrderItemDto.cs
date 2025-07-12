namespace AuthNet.Models.DTO
{
    public class PurchaseOrderItemDto
    {
        public int PurchaseOrderItemId { get; set; }
        public int PurchaseOrderId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
        public string SupplierName { get; set; } = string.Empty;
    }
}
