using AuthNet.Data;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            try
            {
                return await _context.Users
                    .Select(s => new UserViewModel
                    {
                        UserId = s.UserId,
                        Username = s.Username,
                        Role = s.Role,
                        CreatedAt = s.CreatedAt
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<UserViewModel>();
            }
        }
    }
}
