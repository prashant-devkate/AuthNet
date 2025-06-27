using AuthNet.Models.Domain;
using AuthNet.Models.DTO;

namespace AuthNet.Services.Interfaces
{
    public interface IInvoiceTemplateService
    {
        Task<IEnumerable<InvoiceTemplate>> GetAllAsync();
        Task<InvoiceTemplateDto?> GetByIdAsync(int id);
        Task<int> GetInvoiceTemplateCountAsync();
        Task<OperationResponse> AddAsync(InvoiceTemplate template);
        Task<OperationResponse> UpdateAsync(int id, InvoiceTemplate template);
        Task<OperationResponse> DeleteAsync(int id);
    }
}
