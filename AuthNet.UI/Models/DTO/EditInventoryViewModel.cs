using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditInventoryViewModel : AddInventoryViewModel
    {
        [Required]
        public new int InventoryId { get; set; }
    }
}
