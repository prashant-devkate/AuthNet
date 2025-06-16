using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.ToListAsync();

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<Product> AddAsync(Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
