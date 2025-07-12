using AuthNet.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.DTO
{
    public class PurchaseViewModel
    {
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public int? CreatedByUserId { get; set; }

    }
}
