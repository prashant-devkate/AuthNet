using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddSupplierViewModel
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Contact name is required")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
