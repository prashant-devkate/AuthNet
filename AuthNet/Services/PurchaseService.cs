using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _context;
        public PurchaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseViewModel>> GetAllAsync()
        {
            try
            {
                return await _context.purchaseOrders
                    .Select(s => new PurchaseViewModel
                    {
                        PurchaseOrderId = s.PurchaseOrderId,
                        SupplierId = s.SupplierId,
                        CreatedByUserId = s.CreatedByUserId,
                        OrderDate = s.OrderDate,
                        TotalAmount = s.TotalAmount
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<PurchaseViewModel>();
            }
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.purchaseOrders
                    .Include(s => s.OrderItems)
                    .FirstOrDefaultAsync(s => s.PurchaseOrderId == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> GetPurchaseOrderCountAsync()
        {
            return await _context.purchaseOrders.CountAsync();
        }

        public async Task<OperationResponse> AddAsync(PurchaseOrder purchase)
        {
            try
            {
                _context.purchaseOrders.Add(purchase);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = $"Purchase order #{purchase.PurchaseOrderId} added successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Error adding purchase order: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse?> UpdateAsync(int id, PurchaseOrder purchase)
        {
            var existing = await _context.purchaseOrders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.PurchaseOrderId == id);

            if (existing == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Purchase order with ID {id} not found."
                };
            }

            try
            {
                existing.SupplierId = purchase.SupplierId;
                existing.CreatedByUserId = purchase.CreatedByUserId;
                existing.OrderDate = purchase.OrderDate;
                existing.TotalAmount = purchase.OrderItems.Sum(i => i.UnitPrice * i.Quantity);

                var incomingItemIds = purchase.OrderItems?.Where(i => i.PurchaseOrderItemId != 0).Select(i => i.PurchaseOrderItemId).ToList() ?? new List<int>();

                var removedItems = existing.OrderItems.Where(existingItem => !incomingItemIds.Contains(existingItem.PurchaseOrderItemId)).ToList();

                _context.PurchaseOrderItems.RemoveRange(removedItems);

                foreach (var item in purchase.OrderItems ?? Enumerable.Empty<PurchaseOrderItems>())
                {
                    if (item.PurchaseOrderItemId != 0)
                    {
                        var existingItem = existing.OrderItems
                            .FirstOrDefault(oi => oi.PurchaseOrderItemId == item.PurchaseOrderItemId);

                        if (existingItem != null)
                        {
                            existingItem.ProductId = item.ProductId;
                            existingItem.Quantity = item.Quantity;
                            existingItem.UnitPrice = item.UnitPrice;
                        }
                    }
                    else
                    {
                        existing.OrderItems.Add(new PurchaseOrderItems
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Purchase order updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Error updating purchase order: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse> DeleteAsync(int id)
        {
            var purchase = await _context.purchaseOrders.FindAsync(id);
            if (purchase == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Purchase order with ID {id} not found."
                };
            }

            try
            {
                _context.purchaseOrders.Remove(purchase);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Purchase order deleted successfully."
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = "Delete failed: The purchase order is referenced by other records."
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
