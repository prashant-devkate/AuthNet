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
            try
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
            catch(Exception ex)
            {
                return Enumerable.Empty<SupplierViewModel>();
            }
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Suppliers
                    .Include(s => s.Products)
                    .FirstOrDefaultAsync(s => s.SupplierId == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> GetSupplierCountAsync()
        {
            try
            {
                return await _context.Suppliers.CountAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<(Supplier? supplier, OperationResponse response)> AddAsync(Supplier supplier)
        {
            try
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                return (supplier, new OperationResponse
                {
                    Success = true,
                    Message = "Supplier added successfully."
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
        public async Task<(Supplier? supplier, OperationResponse response)> UpdateAsync(int id, Supplier supplier)
        {
            var existing = await _context.Suppliers.FindAsync(id);
            if (existing == null)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = $"Supplier with ID {id} not found."
                });
            }
            try
            {
                _context.Entry(existing).CurrentValues.SetValues(supplier);
                await _context.SaveChangesAsync();

                return (existing, new OperationResponse
                {
                    Success = true,
                    Message = $"Supplier with ID {id} updated successfully."
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return (null, new OperationResponse
                {
                    Success = false,
                    Message = "Update failed: The Supplier is referenced by other records and cannot be updated due to foreign key constraints."
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
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Supplier with ID {id} not found."
                };
            }

            try
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true,
                    Message = $"Supplier with ID {id} was successfully deleted."
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = "Delete failed: The Supplier is referenced by other records (e.g., Products or PurchaseOrders)."
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
