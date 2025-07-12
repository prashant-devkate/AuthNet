using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class PurchaseItemService : IPurchaseItemService
    {
        private readonly AppDbContext _context;
        public PurchaseItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrderItemDto>> GetAllAsync()
        {
            return await _context.PurchaseOrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Supplier)
                .AsNoTracking()
                .Select(oi => new PurchaseOrderItemDto
                {
                    PurchaseOrderItemId = oi.PurchaseOrderItemId,
                    PurchaseOrderId = oi.PurchaseOrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    SupplierName = oi.Order != null && oi.Order.Supplier != null ? oi.Order.Supplier.CompanyName : string.Empty
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrderWithItemsDto>> GetGroupedPurchaseOrderItemsAsync()
        {
            var data = await _context.PurchaseOrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Supplier)
                .AsNoTracking()
                .ToListAsync();

            var grouped = data
                .GroupBy(oi => oi.PurchaseOrderId)
                .Select(group => new PurchaseOrderWithItemsDto
                {
                    PurchaseOrderId = group.Key,
                    SupplierName = group.First().Order?.Supplier?.CompanyName ?? string.Empty,
                    OrderDate = group.First().Order?.OrderDate ?? DateTime.MinValue,
                    OrderItems = group.Select(oi => new PurchaseOrderItemDto
                    {
                        PurchaseOrderItemId = oi.PurchaseOrderItemId,
                        PurchaseOrderId = oi.PurchaseOrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? string.Empty,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                });

            return grouped;
        }


        public async Task<PurchaseOrderItemDto?> GetByIdAsync(int id)
        {
            return await _context.PurchaseOrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Supplier)
                .AsNoTracking()
                .Where(oi => oi.PurchaseOrderItemId == id)
                .Select(oi => new PurchaseOrderItemDto
                {
                    PurchaseOrderItemId = oi.PurchaseOrderItemId,
                    PurchaseOrderId = oi.PurchaseOrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    SupplierName = oi.Order != null && oi.Order.Supplier != null ? oi.Order.Supplier.CompanyName : string.Empty
                })
                .FirstOrDefaultAsync();
        }


        public async Task<PurchaseOrderItems> AddAsync(PurchaseOrderItems item)
        {
            _context.PurchaseOrderItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<PurchaseOrderItems?> UpdateAsync(int id, PurchaseOrderItems item)
        {
            var existing = await _context.PurchaseOrderItems.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.PurchaseOrderItems.FindAsync(id);
            if (item == null) return false;

            _context.PurchaseOrderItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
