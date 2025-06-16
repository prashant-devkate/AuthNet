namespace AuthNet.Models.Domain
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
