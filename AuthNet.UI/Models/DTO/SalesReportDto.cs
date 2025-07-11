namespace AuthNet.UI.Models.DTO
{
    public class SalesReportDto
    {
        public string Title { get; set; }
        public decimal TotalSales { get; set; }
        public int InvoiceCount { get; set; }
        public List<SaleSummaryDto> Sales { get; set; }
    }

    public class SaleSummaryDto
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemSummaryDto> Items { get; set; }
    }

    public class SaleItemSummaryDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
