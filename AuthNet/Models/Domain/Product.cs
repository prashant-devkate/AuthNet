using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.Domain
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SellPrice { get; set; }
        public int ProductKey { get; set; }
        public string? ProductCode { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        //public Inventory? Inventory { get; set; }
        //public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
