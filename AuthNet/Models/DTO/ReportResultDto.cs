namespace AuthNet.Models.DTO
{
    public class ReportResultDto
    {
        public string Title { get; set; }
        public decimal TotalSales { get; set; }
        public int InvoiceCount { get; set; }
        public List<ReportInvoiceDto> Sales { get; set; }
    }

    public class ReportInvoiceDto
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ReportSaleItemDto> Items { get; set; }
    }

    public class ReportSaleItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
