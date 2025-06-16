using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Category>> GetAllAsync() => await _context.Categories.ToListAsync();

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<int> GetCategoryCountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> UpdateAsync(int id, Category category)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
