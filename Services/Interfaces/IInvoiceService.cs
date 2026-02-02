using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;
using ASP_NET_Final_Proj.Models;

namespace ASP_NET_Final_Proj.Services.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
    Task<InvoiceResponseDto> GetByIdAsync(Guid id);
    Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto createdInvoiceRequest);
    Task<InvoiceResponseDto> EditAsync(Guid id, EditInvoiceDto updatedInvoiceRequest);
    Task<InvoiceResponseDto> ChangeStatusAsync(Guid id, InvoiceStatus status);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(InvoiceQueryParams queryParams);
}
