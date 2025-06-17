using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.Domain
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
