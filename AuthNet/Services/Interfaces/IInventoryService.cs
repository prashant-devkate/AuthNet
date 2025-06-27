using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<InventoryDto?> GetByIdAsync(int id);
        Task<int> GetInventoryCountAsync();
        Task<OperationResponse> AddAsync(Inventory inventory);
        Task<OperationResponse> UpdateAsync(int id, Inventory inventory);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
