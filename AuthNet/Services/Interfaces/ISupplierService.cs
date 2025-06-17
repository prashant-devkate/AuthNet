using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierViewModel>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<int> GetSupplierCountAsync();
        Task<(Supplier? supplier, OperationResponse response)> AddAsync(Supplier supplier);
        Task<(Supplier? supplier, OperationResponse response)> UpdateAsync(int id, Supplier supplier);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
