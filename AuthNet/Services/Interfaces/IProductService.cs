using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<int> GetProductCountAsync();
        Task<OperationResponse> AddAsync(Product product);
        Task<OperationResponse> UpdateAsync(int id, Product product);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
