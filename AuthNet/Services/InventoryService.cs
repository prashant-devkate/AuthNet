using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class InventoryService :IInventoryService
    {
        private readonly AppDbContext _context;
        public InventoryService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Inventory>> GetAllAsync() =>
            await _context.Inventories.Include(i => i.Product).ToListAsync();

        public async Task<Inventory?> GetByProductIdAsync(int productId) =>
            await _context.Inventories.Include(i => i.Product).FirstOrDefaultAsync(i => i.ProductId == productId);

        public async Task<Inventory> AddOrUpdateAsync(Inventory inventory)
        {
            var existing = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == inventory.ProductId);
            if (existing == null)
            {
                _context.Inventories.Add(inventory);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(inventory);
            }

            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inventory == null) return false;

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
