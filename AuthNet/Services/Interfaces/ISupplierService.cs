using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierViewModel>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<int> GetSupplierCountAsync();
        Task<Supplier> AddAsync(Supplier supplier);
        Task<Supplier?> UpdateAsync(int id, Supplier supplier);
        Task<DeleteResponse> DeleteAsync(int id);
    }
}
