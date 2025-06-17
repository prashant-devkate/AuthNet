using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllAsync();
    }
}
