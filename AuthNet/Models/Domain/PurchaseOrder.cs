using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.Domain
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }
        [Required]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public ICollection<PurchaseOrderItems>? OrderItems { get; set; }
    }
}
