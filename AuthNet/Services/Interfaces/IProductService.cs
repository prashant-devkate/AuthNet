using AuthNet.Models;

namespace AuthNet.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByProductCodeAsync(string productCode);
        Task<ServiceResponse<int>> CreateAsync(Product product);
        Task<ServiceResponse<Product>> UpdateAsync(Product product);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
}
