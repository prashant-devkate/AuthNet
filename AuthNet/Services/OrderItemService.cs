using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly AppDbContext _context;
        public OrderItemService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<OrderItem>> GetAllAsync() => await _context.OrderItems.Include(o => o.Product).Include(o => o.Order).ToListAsync();

        public async Task<OrderItem?> GetByIdAsync(int id) => await _context.OrderItems.Include(o => o.Product).Include(o => o.Order).FirstOrDefaultAsync(o => o.OrderItemId == id);

        public async Task<OrderItem> AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<OrderItem?> UpdateAsync(int id, OrderItem item)
        {
            var existing = await _context.OrderItems.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.OrderItems.FindAsync(id);
            if (item == null) return false;

            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
