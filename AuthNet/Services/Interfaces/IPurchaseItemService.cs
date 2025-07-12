using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IPurchaseItemService
    {
        Task<IEnumerable<PurchaseOrderItemDto>> GetAllAsync();
        Task<IEnumerable<PurchaseOrderWithItemsDto>> GetGroupedPurchaseOrderItemsAsync();
        Task<PurchaseOrderItemDto?> GetByIdAsync(int id);
        Task<PurchaseOrderItems> AddAsync(PurchaseOrderItems item);
        Task<PurchaseOrderItems?> UpdateAsync(int id, PurchaseOrderItems item);
        Task<bool> DeleteAsync(int id);
    }
}
