using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddOrderViewModel
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Total Amount is required")]
        public decimal TotalAmount { get; set; }
        public List<AddOrderItemViewModel> OrderItems { get; set; } = new();
    }

    public class AddOrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
