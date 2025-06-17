using AuthNet.Data;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllAsync()
        {
            try
            {
                return await _context.Customers
                    .Select(s => new CustomerViewModel
                    {
                        CustomerId = s.CustomerId,
                        Name = s.Name,
                        Email = s.Email,
                        Phone = s.Phone,
                        Address = s.Address
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<CustomerViewModel>();
            }
        }

    }
}
