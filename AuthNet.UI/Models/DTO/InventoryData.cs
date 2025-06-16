namespace AuthNet.UI.Models.DTO
{
    public class InventoryData
    {
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
