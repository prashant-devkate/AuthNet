namespace AuthNet.UI.Models.DTO
{
    public class PurchaseOrderWithItemsViewModel
    {
        public int PurchaseOrderId { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<PurchaseOrderItemViewModel> OrderItems { get; set; } = new();
    }
}
