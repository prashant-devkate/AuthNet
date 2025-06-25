using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class InventoryService :IInventoryService
    {
        private readonly AppDbContext _context;
        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Inventories.ToListAsync();
        }

        public async Task<InventoryDto?> GetByIdAsync(int id)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(c => c.InventoryId == id);
            if (inventory == null) return null;

            return new InventoryDto
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                QuantityInStock = inventory.QuantityInStock,
                ReorderLevel = inventory.ReorderLevel,
                LastUpdated = inventory.LastUpdated
            };
        }

        public async Task<int> GetInventoryCountAsync()
        {
            return await _context.Inventories.CountAsync();
        }

        public async Task<OperationResponse> AddAsync(Inventory inventory)
        {
            try
            {
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Inventory added successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"An error occurred while adding the inventory: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse> UpdateAsync(int id, Inventory inventory)
        {
            var existing = await _context.Inventories.FindAsync(id);
            if (existing == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Inventory with ID {id} not found."
                };
            }

            try
            {
                _context.Entry(existing).CurrentValues.SetValues(inventory);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Inventory updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating the inventory: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse> DeleteAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Inventory with ID {id} not found."
                };
            }

            try
            {
                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Inventory deleted successfully."
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = "Delete failed: The inventory is referenced by other records (e.g., products)."
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
            catch (InvalidOperationException ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Invalid operation: {ex.Message}"
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
