using AuthNet.Models.Domain;

namespace AuthNet.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory?> GetByProductIdAsync(int productId);
        Task<Inventory> AddOrUpdateAsync(Inventory inventory);
        Task<bool> DeleteAsync(int productId);
    }
}
