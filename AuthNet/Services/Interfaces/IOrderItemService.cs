using AuthNet.Models.Domain;

namespace AuthNet.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(int id);
        Task<OrderItem> AddAsync(OrderItem item);
        Task<OrderItem?> UpdateAsync(int id, OrderItem item);
        Task<bool> DeleteAsync(int id);
    }
}
