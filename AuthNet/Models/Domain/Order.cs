namespace AuthNet.Models.Domain
{
    public class Order
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
