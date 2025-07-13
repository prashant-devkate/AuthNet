namespace AuthNet.UI.Models.DTO
{
    public class CompanyInfoDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        public string ContactNumber { get; set; }

        public string? LogoFilePath { get; set; }
        public string? SignFilePath { get; set; }
    }
}
