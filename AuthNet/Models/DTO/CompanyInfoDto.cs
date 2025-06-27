using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.DTO
{
    public class CompanyInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        public IFormFile? LogoUrl { get; set; }
        public string? LogoName { get; set; }
        public IFormFile? SignatureUrl { get; set; }
        public string? SignName { get; set; }
    }
}
