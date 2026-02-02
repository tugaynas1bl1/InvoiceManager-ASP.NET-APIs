using ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.Models;

namespace ASP_NET_Final_Proj.Services.Interfaces;

public interface IInvoiceRowService
{
    Task<IEnumerable<InvoiceRowResponseDto>> GetAllAsync();
    Task<InvoiceRowResponseDto> GetByIdAsync(Guid id);
    Task<InvoiceRowResponseDto> CreateAsync(CreateInvoiceRowDto createdInvoiceRequest);
    Task<InvoiceRowResponseDto> EditAsync(Guid id, EditInvoiceRowDto updatedInvoiceRequest);
    Task<bool> DeleteAsync(Guid id);
}
