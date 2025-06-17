using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderViewModel>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<int> GetOrderCountAsync();
        Task<(Order? order, OperationResponse response)> AddAsync(Order order);
        Task<(Order? order, OperationResponse response)> UpdateAsync(int id, Order order);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
