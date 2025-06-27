namespace AuthNet.UI.Models.DTO
{
    public class ProductListViewModel
    {
        public List<ProductDto> Products { get; set; }
        public List<InvoiceTemplateDto>? Invoices { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
