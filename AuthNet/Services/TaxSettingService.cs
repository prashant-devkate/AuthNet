using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Services
{
    public class TaxSettingService : ITaxSettingService
    {
        private readonly AppDbContext _context;
        public TaxSettingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaxSettingResponseDto> GetAllAsync()
        {
            var tax = await _context.TaxSettings.FirstOrDefaultAsync();

            if (tax == null)
                return null;

            return new TaxSettingResponseDto
            {
                Id = tax.Id,
                CGST = tax.CGST,
                SGST = tax.SGST,
                IGST = tax.IGST
            };
        }

        public async Task<TaxSettingDto?> GetByIdAsync(int id)
        {
            var tax = await _context.TaxSettings.FirstOrDefaultAsync(c => c.Id == id);
            if (tax == null) return null;

            return new TaxSettingDto
            {
                Id = tax.Id,
                CGST = tax.CGST,
                SGST = tax.SGST,
                IGST = tax.IGST
            };
        }
        public async Task<OperationResponse> AddAsync(TaxSetting dto)
        {
            try
            {
                var exists = await _context.TaxSettings.AnyAsync();
                if (exists)
                {
                    return new OperationResponse
                    {
                        Success = false,
                        Message = "Tax settings already exists. Use update instead."
                    };
                }

                var tax = new TaxSetting
                {
                    CGST = dto.CGST,
                    SGST = dto.SGST,
                    IGST = dto.IGST
                };

                _context.TaxSettings.Add(tax);
                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true,
                    Message = "Tax settings saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = "An error occurred while saving tax settings."
                };
            }
        }

        public async Task<OperationResponse> UpdateAsync(int id, TaxSetting taxSetting)
        {
            var existing = await _context.TaxSettings.FindAsync(id);
            if (existing == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Category with ID {id} not found."
                };
            }

            try
            {
                _context.Entry(existing).CurrentValues.SetValues(taxSetting);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Tax settings updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating the tax settings: {ex.Message}"
                };
            }
        }
    }
}
