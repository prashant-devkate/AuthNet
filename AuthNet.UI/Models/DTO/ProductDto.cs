using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int ProductKey { get; set; }
        public string ProductCode { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        //public InventoryData? Inventory { get; set; }
    }
}
