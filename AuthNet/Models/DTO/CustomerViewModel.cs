using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.DTO
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
