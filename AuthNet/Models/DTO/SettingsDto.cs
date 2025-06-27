namespace AuthNet.Models.DTO
{
    public class SettingsDto
    {
        public CompanyInfoDto CompanyInfo { get; set; }
        public InvoiceTemplateDto InvoiceTemplate { get; set; }
        public TaxSettingsDto TaxSettings { get; set; }
    }
}
