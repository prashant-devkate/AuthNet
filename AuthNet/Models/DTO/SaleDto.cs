namespace AuthNet.Models.DTO
{
    public class SaleDto
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal DiscountedAmount { get; set; }
        public decimal AfterTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Template { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<SaleItemDto> SaleItems { get; set; }
    }
}
