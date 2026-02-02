using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;

namespace ASP_NET_Final_Proj.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetAllAsync();
    Task<CustomerResponseDto> GetByIdAsync(Guid id);
    Task<CustomerResponseDto> AddAsync(CreateCustomerDto createdCustomerRequest);
    Task<CustomerResponseDto> EditAsync(Guid id, EditCustomerDto edittedCustomerRequest);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    Task<PagedResult<CustomerResponseDto>> GetPagedAsync (CustomerQueryParams queryParams);
}
