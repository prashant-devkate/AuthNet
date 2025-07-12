using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderViewModel>> GetAllAsync()
        {
            try
            {
                return await _context.Orders
                    .Select(s => new OrderViewModel
                    {
                        OrderId = s.OrderId,
                        CustomerId = s.CustomerId,
                        UserId = s.UserId,
                        OrderDate = s.OrderDate,
                        TotalAmount = s.TotalAmount
                    }).ToListAsync();
    }
            catch (Exception ex)
            {
                return Enumerable.Empty<OrderViewModel>();
            }
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Orders
                    .Include(s => s.OrderItems)
                    .FirstOrDefaultAsync(s => s.OrderId == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> GetOrderCountAsync()
        {
            try
            {
                return await _context.Orders.CountAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<(Order? order, OperationResponse response)> AddAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return (order, new OperationResponse
                {
                    Success = true,
                    Message = "Order added successfully."
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = "Add failed: Foreign key constraint violation."
                });
            }
            catch (DbUpdateException ex)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"Database update error: {ex.Message}"
                });
            }
            catch (InvalidOperationException ex)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"Invalid operation: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }

        }
        public async Task<(Order? order, OperationResponse response)> UpdateAsync(int id, Order order)
        {
            var existing = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);
            if (existing == null)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"Order with ID {id} not found."
                });
            }
            try
            {
                existing.CustomerId = order.CustomerId;
                existing.UserId = order.UserId;
                existing.OrderDate = order.OrderDate;
                existing.TotalAmount = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);

                var incomingItemIds = order.OrderItems?.Select(oi => oi.OrderItemId).ToList() ?? new List<int>();
                var removedItems = existing.OrderItems.Where(existingItem => !incomingItemIds.Contains(existingItem.OrderItemId)).ToList();

                _context.OrderItems.RemoveRange(removedItems);

                foreach (var item in order.OrderItems ?? Enumerable.Empty<OrderItem>())
                {
                    var existingItem = existing.OrderItems.FirstOrDefault(oi => oi.OrderItemId == item.OrderItemId);

                    if (existingItem != null)
                    {
                        // Update
                        existingItem.ProductId = item.ProductId;
                        existingItem.Quantity = item.Quantity;
                        existingItem.UnitPrice = item.UnitPrice;
                    }
                    else
                    {
                        // Add new item
                        existing.OrderItems.Add(new OrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return (existing, new OperationResponse
                {
                    Success = true,
                    Message = $"Order with ID {id} updated successfully."
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = "Update failed: The Order is referenced by other records and cannot be updated due to foreign key constraints."
                });
            }
            catch (DbUpdateException ex)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"Database update error: {ex.Message}"
                });
            }
            catch (InvalidOperationException ex)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"Invalid operation: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }

        }

        public async Task<OperationResponse> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Order with ID {id} not found."
                };
            }

            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true,
                    Message = $"Order with ID {id} was successfully deleted."
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = "Delete failed: The Order is referenced by other records (e.g., Products or OrderItems)."
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
