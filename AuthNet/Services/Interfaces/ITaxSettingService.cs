using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ITaxSettingService
    {
        Task<TaxSettingResponseDto> GetAllAsync();
        Task<TaxSettingDto?> GetByIdAsync(int id);
        Task<OperationResponse> AddAsync(TaxSetting taxSetting);
        Task<OperationResponse> UpdateAsync(int id, TaxSetting taxSetting);
    }
}
