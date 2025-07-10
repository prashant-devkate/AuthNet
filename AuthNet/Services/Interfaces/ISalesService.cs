using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ISalesService
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task<int> GetSaleCountAsync();
        Task<OperationResponse> AddAsync(Sale sale);
        Task<OperationResponse> UpdateAsync(int id, Sale sale);
        Task<OperationResponse> DeleteAsync(int id);
        Task<ReportResultDto> GetDailySalesAsync();
        Task<ReportResultDto> GetMonthlySalesAsync();
        Task<ReportResultDto> GetYearlySalesAsync();
    }
}
