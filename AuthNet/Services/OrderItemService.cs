using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly AppDbContext _context;
        public OrderItemService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
                .AsNoTracking()
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    CustomerName = oi.Order != null && oi.Order.Customer != null ? oi.Order.Customer.Name : string.Empty
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderWithItemsDto>> GetGroupedOrderItemsAsync()
        {
            var data = await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
                .AsNoTracking()
                .ToListAsync();

            var grouped = data
                .GroupBy(oi => oi.OrderId)
                .Select(group => new OrderWithItemsDto
                {
                    OrderId = group.Key,
                    CustomerName = group.First().Order?.Customer?.Name ?? string.Empty,
                    OrderDate = group.First().Order?.OrderDate ?? DateTime.MinValue,
                    OrderItems = group.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? string.Empty,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                });

            return grouped;
        }


        public async Task<OrderItemDto?> GetByIdAsync(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
                .AsNoTracking()
                .Where(oi => oi.OrderItemId == id)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    CustomerName = oi.Order != null && oi.Order.Customer != null ? oi.Order.Customer.Name : string.Empty
                })
                .FirstOrDefaultAsync();
        }


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
