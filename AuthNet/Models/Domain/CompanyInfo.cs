using System.ComponentModel.DataAnnotations.Schema;

namespace AuthNet.Models.Domain
{
    public class CompanyInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        public string ContactNumber { get; set; }

        [NotMapped]
        public IFormFile? LogoUrl { get; set; }
        public string? LogoName { get; set; }
        public string LogoFileExtension { get; set; }
        public long LogoFileSizeInBytes { get; set; }
        public string LogoFilePath { get; set; }

        [NotMapped]
        public IFormFile? SignatureUrl { get; set; }
        public string? SignName { get; set; }
        public string SignFileExtension { get; set; }
        public long SignFileSizeInBytes { get; set; }
        public string SignFilePath { get; set; }
    }
}
