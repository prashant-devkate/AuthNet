using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface ISalesService
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task<int> GetSaleCountAsync();
        Task<OperationResponse> AddAsync(SaleDto sale);
        Task<OperationResponse> UpdateAsync(int id, Sale sale);
        Task<OperationResponse> DeleteAsync(int id);
        Task<ReportResultDto> GetDailySalesAsync();
        Task<ReportResultDto> GetMonthlySalesAsync();
        Task<ReportResultDto> GetYearlySalesAsync();
        Task<DailyProfitDto> CalculateDailyProfitAsync();
        Task<List<DailyProfitDto>> CalculateWeeklyProfitAsync();
        Task<List<MonthlyProfitDto>> CalculateMonthlyProfitAsync(int? year = null);
        Task<List<HalfYearlyProfitDto>> CalculateHalfYearlyProfitAsync(int? year = null);
        Task<List<YearlyProfitDto>> CalculateYearlyProfitAsync();
    }
}
