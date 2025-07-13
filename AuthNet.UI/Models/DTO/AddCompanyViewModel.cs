using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddCompanyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company/Shop name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Company/Shop address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "GST number is required")]
        public string GSTIN { get; set; }

        [Phone, Required(ErrorMessage = "Phone number is required")]
        public string ContactNumber { get; set; }
        public IFormFile? LogoUrl { get; set; }
        public IFormFile? SignatureUrl { get; set; }

        public string? LogoFilePath { get; set; }
        public string? SignFilePath { get; set; }
    }
}
