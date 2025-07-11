using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddCustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer name is required"), StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required"),EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required"), Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Category name is required"), StringLength(500)]
        public string Address { get; set; }
    }
}
