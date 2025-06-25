using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<OperationResponse> SaveCompanyInfoAsync(CompanyInfoDto dto, HttpRequest request);
        Task<CompanyInfoResponseDto> GetCompanyInfoAsync();
        Task<OperationResponse> UpdateCompanyInfoAsync(CompanyInfoDto dto, HttpRequest request);
        Task<bool> SoftResetCompanyInfoAsync();
        Task<List<CompanyInfoHistoryDto>> GetCompanyHistoryInfoAsync();

    }
}
