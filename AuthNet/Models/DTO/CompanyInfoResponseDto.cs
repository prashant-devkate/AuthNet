namespace AuthNet.Models.DTO
{
    public class CompanyInfoResponseDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        public string ContactNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? SignatureUrl { get; set; }
    }
}
