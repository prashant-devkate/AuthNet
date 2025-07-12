using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddPurchaseOrderViewModel
    {
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Total Amount is required")]
        public decimal TotalAmount { get; set; }
        public List<AddPurchaseOrderItemViewModel> OrderItems { get; set; } = new();
    }

    public class AddPurchaseOrderItemViewModel
    {
        public int PurchaseOrderItemId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
