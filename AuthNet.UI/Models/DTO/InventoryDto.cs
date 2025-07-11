using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "Quantity")]
        public int QuantityInStock { get; set; }

        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ProductName { get; set; }
    }
}
