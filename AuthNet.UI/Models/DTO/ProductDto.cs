using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        [StringLength(1, ErrorMessage = "Only one character is allowed.")]
        [RegularExpression("^[A-Za-z]$", ErrorMessage = "Only a single letter (A-Z or a-z) is allowed.")]
        public string HotKey { get; set; }
        public string ProductCode { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        //public InventoryData? Inventory { get; set; }
    }
}
