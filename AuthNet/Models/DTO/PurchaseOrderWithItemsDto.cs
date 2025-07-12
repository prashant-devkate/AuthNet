namespace AuthNet.Models.DTO
{
    public class PurchaseOrderWithItemsDto
    {
        public int PurchaseOrderId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }

        public List<PurchaseOrderItemDto> OrderItems { get; set; } = new();
    }
}
