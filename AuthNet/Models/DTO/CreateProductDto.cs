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

        [Required]
        public string ProductCode { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SupplierId { get; set; }
    }
}
