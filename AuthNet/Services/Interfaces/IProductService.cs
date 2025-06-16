using AuthNet.Models.Domain;

namespace AuthNet.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<int> GetProductCountAsync();
        Task<Product> AddAsync(Product product);
        Task<Product?> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
    }
}
