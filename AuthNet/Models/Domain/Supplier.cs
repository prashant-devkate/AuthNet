using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.Domain
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
