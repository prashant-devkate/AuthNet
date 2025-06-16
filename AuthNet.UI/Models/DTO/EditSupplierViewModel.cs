using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditSupplierViewModel : AddSupplierViewModel
    {
        [Required]
        public new int SupplierId { get; set; }
    }
}
