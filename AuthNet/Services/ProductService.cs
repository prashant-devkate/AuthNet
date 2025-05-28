using AuthNet.Data;
using AuthNet.Models;
using AuthNet.Services.Interfaces;
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

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> GetByProductCodeAsync(string productCode)
        {
            return await _context.Products.FindAsync(productCode);
        }

        public async Task<ServiceResponse<int>> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return new ServiceResponse<int>
            {
                Message = "Product created successfully.",
                Data = product.Id
            };
        }

        public async Task<ServiceResponse<Product>> UpdateAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing == null)
            {
                return new ServiceResponse<Product>
                {
                    Status = "Failed",
                    Message = "Product not found.",
                    Data = null
                };
            }

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Quantity = product.Quantity;
            existing.Price = product.Price;
            existing.ProductCode = product.ProductCode;

            await _context.SaveChangesAsync();

            return new ServiceResponse<Product>
            {
                Message = "Product updated successfully.",
                Data = existing
            };
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new ServiceResponse<bool>
                {
                    Status = "Failed",
                    Message = "Product not found.",
                    Data = false
                };
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Message = "Product deleted successfully.",
                Data = true
            };
        }
    }
}
