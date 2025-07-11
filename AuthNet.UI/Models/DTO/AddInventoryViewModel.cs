using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddInventoryViewModel
    {
        public int? InventoryId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage ="Quantity is required")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage ="Reorder level is required")]
        public int ReorderLevel { get; set; } 
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
