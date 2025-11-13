using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.Domain
{
    public class Sale
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }

        [StringLength(100)]
        public string Customername { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public decimal PrincipalAmount { get; set; }
        public decimal DiscountedAmount { get; set; }
        public decimal AfterTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Template { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; }
    }
}
