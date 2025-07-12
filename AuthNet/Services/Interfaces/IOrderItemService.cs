using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetAllAsync();
        Task<IEnumerable<OrderWithItemsDto>> GetGroupedOrderItemsAsync();
        Task<OrderItemDto?> GetByIdAsync(int id);
        Task<OrderItem> AddAsync(OrderItem item);
        Task<OrderItem?> UpdateAsync(int id, OrderItem item);
        Task<bool> DeleteAsync(int id);
    }
}
