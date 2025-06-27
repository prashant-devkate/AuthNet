using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.DTO
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required,RegularExpression(@"^[A-Za-z0-9]$", 
            ErrorMessage = "Enter exactly one letter or one digit.")]
        public string HotKey { get; set; }

        [Required]
        public string ProductCode { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SupplierId { get; set; }
    }
}
