using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddProductViewModel
    {
        public int ProductId { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        [Required] public string HotKey { get; set; } = string.Empty;
        [Required] public string ProductCode { get; set; } = string.Empty;
        [Required] public int? CategoryId { get; set; }
        [Required] public int? SupplierId { get; set; }

        //public InventoryData Inventory { get; set; } = new();
    }
}
