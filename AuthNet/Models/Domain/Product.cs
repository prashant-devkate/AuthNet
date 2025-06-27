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
        public decimal Price { get; set; }
        [RegularExpression(@"^[A-Za-z0-9]$",
            ErrorMessage = "Enter exactly one letter or one digit.")]
        public string HotKey { get; set; }
        public string? ProductCode { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        //public Inventory? Inventory { get; set; }
        //public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
