using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddProductViewModel
    {
        public int ProductId { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;

        [Required]
        public decimal CostPrice { get; set; }
        [Required]
        public decimal SellPrice { get; set; }
        [Required] public int ProductKey { get; set; }
        [Required] public string ProductCode { get; set; } = string.Empty;
        public string Barcode { get; set; }
        [Required] public int? CategoryId { get; set; }
        [Required] public int? SupplierId { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }

        //public InventoryData Inventory { get; set; } = new();
    }
}
