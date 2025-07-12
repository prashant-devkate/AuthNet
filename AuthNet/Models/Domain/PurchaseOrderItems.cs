using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.Domain
{
    public class PurchaseOrderItems
    {
        [Key]
        public int PurchaseOrderItemId { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? Order { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
