using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;
        public SupplierService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<SupplierViewModel>> GetAllAsync()
        {
            return await _context.Suppliers
                .Select(s => new SupplierViewModel
                {
                    SupplierId = s.SupplierId,
                    CompanyName = s.CompanyName,
                    ContactName = s.ContactName,
                    Phone = s.Phone,
                    Address = s.Address
                }).ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id) => await _context.Suppliers.Include(s => s.Products).FirstOrDefaultAsync(s => s.SupplierId == id);

        public async Task<int> GetSupplierCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        public async Task<Supplier> AddAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier?> UpdateAsync(int id, Supplier supplier)
        {
            var existing = await _context.Suppliers.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(supplier);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<DeleteResponse> DeleteAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return new DeleteResponse
                {
                    Success = false,
                    Message = $"Supplier with {id} not found."
                };
            }
            try
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                return new DeleteResponse
                {
                    Success = true,
                    Message = $"Supplier with {id} was successfully deleted."
                };
            }
            catch(DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return new DeleteResponse
                {
                    Success = false,
                    Message = "Delete Failed: The Supplier is referenced by other records (e.g., Products or PurchaseOrders)."
                };
            }
        }
    }
}
