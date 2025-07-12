using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditPurchaseOrderViewModel : AddPurchaseOrderViewModel
    {
        [Required]
        public new int PurchaseOrderId { get; set; }
    }
}
