namespace AuthNet.Models.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int ProductKey { get; set; }
        public string ProductCode { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public string? CategoryName { get; set; }
        public string? SupplierCompanyName { get; set; }
    }
}
