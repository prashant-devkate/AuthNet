using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<int> GetCategoryCountAsync();
        Task<OperationResponse> AddAsync(Category category);
        Task<OperationResponse> UpdateAsync(int id, Category category);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
