using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddCompanyInfoViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string GSTIN { get; set; }

        public IFormFile? LogoUrl { get; set; }
        public IFormFile? SignatureUrl { get; set; }

        public string? LogoFilePath { get; set; }
        public string? SignFilePath { get; set; }
    }
}
