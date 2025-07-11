using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<int> GetCustomerCountAsync();
        Task<OperationResponse> AddAsync(Customer customer);
        Task<OperationResponse> UpdateAsync(int id, Customer customer);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
