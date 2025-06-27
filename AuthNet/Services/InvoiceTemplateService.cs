using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class InvoiceTemplateService :IInvoiceTemplateService
    {
        private readonly AppDbContext _context;
        public InvoiceTemplateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceTemplate>> GetAllAsync()
        {
            return await _context.InvoiceTemplates.ToListAsync();
        }

        public async Task<InvoiceTemplateDto?> GetByIdAsync(int id)
        {
            var invoiceTemplate = await _context.InvoiceTemplates.FirstOrDefaultAsync(c => c.Id == id);
            if (invoiceTemplate == null) return null;

            return new InvoiceTemplateDto
            {
                Id = invoiceTemplate.Id,
                Layout = invoiceTemplate.Layout,
                Notes = invoiceTemplate.Notes,
                TermsAndConditions = invoiceTemplate.TermsAndConditions

            };
        }

        public async Task<int> GetInvoiceTemplateCountAsync()
        {
            return await _context.InvoiceTemplates.CountAsync();
        }

        public async Task<OperationResponse> AddAsync(InvoiceTemplate template)
        {
            try
            {
                _context.InvoiceTemplates.Add(template);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Invoice template added successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"An error occurred while adding the invoice template: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse> UpdateAsync(int id, InvoiceTemplate template)
        {
            var existing = await _context.InvoiceTemplates.FindAsync(id);
            if (existing == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Invoice template with ID {id} not found."
                };
            }

            try
            {
                _context.Entry(existing).CurrentValues.SetValues(template);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Invoice template updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating the invoice template: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse> DeleteAsync(int id)
        {
            var template = await _context.InvoiceTemplates.FindAsync(id);
            if (template == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Invoice template with ID {id} not found."
                };
            }

            try
            {
                _context.InvoiceTemplates.Remove(template);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Invoice template deleted successfully."
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
