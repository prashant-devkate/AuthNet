namespace AuthNet.UI.Models.DTO
{
    public class ProductListViewModel
    {
        public List<ProductDto> Products { get; set; }
        public List<InvoiceTemplateDto>? Invoices { get; set; }
        public List<CategoryViewModel>? Cats { get; set; }
        public CompanyInfoDto? company { get; set; }
        public TaxDto? tax { get; set; }
    }
}
