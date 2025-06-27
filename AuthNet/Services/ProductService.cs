using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
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

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<OperationResponse> AddAsync(Product product)
        {
            try
            {
                bool hotKeyTaken = await _context.Products
                                         .AnyAsync(p => p.HotKey == product.HotKey);

                if (hotKeyTaken)
                {
                    return new OperationResponse
                    {
                        Success = false,
                        Message = $"HotKey '{product.HotKey}' is already in use. " +
                                  "Choose a different one."
                    };
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Product added successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Error adding product: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse?> UpdateAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Product with ID {id} not found."
                };
            }

            if (!string.Equals(existing.HotKey, product.HotKey,
                       StringComparison.OrdinalIgnoreCase))
            {
                bool hotKeyTaken = await _context.Products
                                                 .AnyAsync(p =>
                                                     p.ProductId != id &&
                                                     p.HotKey == product.HotKey);

                if (hotKeyTaken)
                {
                    return new OperationResponse
                    {
                        Success = false,
                        Message = $"HotKey '{product.HotKey}' is already in use. " +
                                  "Choose a different one."
                    };
                }
            }

            _context.Entry(existing).CurrentValues.SetValues(product);

            try
            {
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Product updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Error updating product: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Product with ID {id} not found."
                };
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Product deleted successfully."
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = "Delete failed: The product is referenced by other records (e.g., orders or inventory)."
                };
            }
            catch (DbUpdateException ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Database update error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }
    }
}
