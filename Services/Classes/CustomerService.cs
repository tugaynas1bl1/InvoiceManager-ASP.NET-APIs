using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_Final_Proj.Services.Classes;

public class CustomerService : ICustomerService
{
    private readonly InvoiceManagerDbContext _context;
    private readonly IMapper _mapper;

    public CustomerService(InvoiceManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<CustomerResponseDto> AddAsync(CreateCustomerDto createdCustomerRequest)
    {
        var customer = _mapper.Map<Customer>(createdCustomerRequest);

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var customer = await _context
            .Customers
            .Include(c => c.Invoices)
            .FirstAsync(c => c.Id == id);

        if (customer is null || customer.DeletedAt is not null) return false;

        customer.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _context
            .Customers
            .Include(c => c.Invoices)
            .FirstAsync(c => c.Id == id);

        if (customer is null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CustomerResponseDto> EditAsync(Guid id, EditCustomerDto edittedCustomerRequest)
    {
        var edittedCustomer = await _context
            .Customers
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c  => c.Id == id);

        if (edittedCustomer is null || edittedCustomer.DeletedAt is not null) return null;

        _mapper.Map(edittedCustomerRequest, edittedCustomer);

        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerResponseDto>(edittedCustomer);
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
    {
        var customers = await _context
            .Customers
            .Include(c => c.Invoices)
            .Where(c => c.DeletedAt == null)
            .ToListAsync();

        return customers.Select(c => _mapper.Map<CustomerResponseDto>(c));
    }

    public async Task<CustomerResponseDto> GetByIdAsync(Guid id)
    {
        var customer = await _context
            .Customers
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer.DeletedAt is not null) return null;

        return _mapper .Map<CustomerResponseDto>(customer);
    }

    public async Task<PagedResult<CustomerResponseDto>> GetPagedAsync(CustomerQueryParams queryParams)
    {
        queryParams.Validate();

        var query = _context
            .Customers
            .Include(c => c.Invoices)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var searchTerm = queryParams.Search;
            query = query.Where(c => c.Name.Contains(searchTerm) 
            || c.Email.Contains(searchTerm)
            || (c.PhoneNumber != null && c.PhoneNumber.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryParams.Sort))
            query = ApplySorting(query, queryParams.Sort, queryParams.SortDirection);
        else if (queryParams.SortDirection == "asc")
            query = query.OrderBy(c => c.CreatedAt);
        else
            query = query.OrderByDescending(c => c.CreatedAt);

        var skip = (queryParams.Page -1 ) * queryParams.Size;

        var tasks = await query
            .Skip(skip)
            .Take(queryParams.Size)
            .Where(c => c.DeletedAt == null)
            .ToListAsync();

        var taskDtos = _mapper.Map<IEnumerable<CustomerResponseDto>>(tasks);

        return PagedResult<CustomerResponseDto>.Create(
            taskDtos,
            queryParams.Page,
            queryParams.Size,
            totalCount
            );
    }

    public IQueryable<Customer> ApplySorting(
        IQueryable<Customer> query,
        string sort,
        string? sortDirection)
    {
        bool isDescending = sortDirection.ToLower() == "desc";

        return sort.ToLower() switch
        {
            "name" => isDescending
                ? query.OrderByDescending(q => q.Name)
                : query.OrderBy(q => q.Name),
            "email" => isDescending
                ? query.OrderByDescending(q => q.Email)
                : query.OrderBy(q => q.Email),
            "phonenumber" => isDescending
                ? query.OrderByDescending(q => q.PhoneNumber)
                : query.OrderBy(q => q.PhoneNumber),
            "createdat" => isDescending
                ? query.OrderByDescending(q => q.CreatedAt)
                : query.OrderBy(q => q.CreatedAt),
            "updatedat" => isDescending
                ? query.OrderByDescending(q => q.UpdatedAt)
                : query.OrderBy(q => q.UpdatedAt),
             _ => query.OrderBy(q => q.CreatedAt)
        };
    }
}
