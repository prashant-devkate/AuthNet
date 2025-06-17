using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllAsync();
    }
}
