using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<int> GetCategoryCountAsync();
        Task<Category> AddAsync(Category category);
        Task<Category?> UpdateAsync(int id, Category category);
        Task<bool> DeleteAsync(int id);
    }
}
