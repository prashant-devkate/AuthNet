using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseViewModel>> GetAllAsync();
        Task<PurchaseOrder?> GetByIdAsync(int id);
        Task<int> GetPurchaseOrderCountAsync();
        Task<OperationResponse> AddAsync(PurchaseOrder purchase);
        Task<OperationResponse> UpdateAsync(int id, PurchaseOrder purchase);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
