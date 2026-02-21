using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_NET_Final_Proj.Services.Classes;

public class InvoiceService : IInvoiceService
{
    private readonly InvoiceManagerDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InvoiceService(InvoiceManagerDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto createdInvoiceRequest)
    {

        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoice = _mapper.Map<Invoice>(createdInvoiceRequest);

        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == createdInvoiceRequest.CustomerId && c.UserId == userId);

        if (customer is null)
            throw new NullReferenceException("Customer couldn't be found or doesn't belong to this user");

        invoice.Customer = customer;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var invoiceWithCustomer = await _context.Invoices
        .Include(i => i.Customer)
        .Include(i => i.Rows)
        .FirstAsync(i => i.Id == invoice.Id);

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return false;

        var invoice = await _context
            .Invoices
            .Include(i => i.Customer)
            .FirstAsync(i => i.Id == id && i.Customer.UserId == userId);

        if (invoice is null || invoice.DeletedAt is not null) 
            return false;

        invoice.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoice = await _context
            .Invoices
            .Include(i => i.Customer)
            .Where(i => i.Status == InvoiceStatus.Created)
            .FirstAsync(i => i.Id == id && i.Customer.UserId == userId);

        if (invoice is null) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<InvoiceResponseDto> EditAsync(Guid id, EditInvoiceDto edittedInvoiceRequest)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var edittedInvoice = await _context
            .Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id && i.Customer.UserId == userId);

        if (edittedInvoice is null || edittedInvoice.DeletedAt is not null) 
            throw new NullReferenceException("Invoice you want to delete does not exist or not belong to this user");

        _mapper.Map(edittedInvoiceRequest, edittedInvoice);

        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceResponseDto>(edittedInvoice);
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllAsync()
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoices = await _context
            .Invoices
            .Include(i => i.Customer)
            .Where(i => i.DeletedAt == null && i.Customer.UserId == userId)
            .ToListAsync();

        if (invoices.Count == 0)
            throw new NullReferenceException();

        return invoices.Select(c => _mapper.Map<InvoiceResponseDto>(c));
    }

    public async Task<InvoiceResponseDto> GetByIdAsync(Guid id)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoice = await _context
            .Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id && i.Customer.UserId == userId);

        if (invoice?.DeletedAt is not null || invoice is null) 
                throw new NullReferenceException();

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto> ChangeStatusAsync(Guid id, InvoiceStatus status)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoice = await _context
            .Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == id && i.Customer.UserId == userId);

        if (invoice is null)
            throw new NullReferenceException();

        invoice.Status = status;
        await _context.SaveChangesAsync();
        
        var item = invoice.DeletedAt != null ? null : _mapper.Map<InvoiceResponseDto>(invoice);

        return item;
    }

    public async Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(InvoiceQueryParams queryParams)
    {
        queryParams.Validate();

        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var query = _context
            .Invoices
            .Include(i => i.Customer)
            .Where(i => i.DeletedAt == null && i.Customer.UserId == userId)
            .AsQueryable();

        if (queryParams.CustomerId.HasValue)
            query = query.Where(i => i.CustomerId == queryParams.CustomerId.Value);

        if (!string.IsNullOrWhiteSpace(queryParams.Status))
        {
            if (Enum.TryParse<InvoiceStatus>(queryParams.Status, out var status))
                query = query.Where(i => i.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var searchTerm = queryParams.Search.ToLower();
            query = query.Where(i => i.Comment != null && i.Comment.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryParams.Sort))
            query = ApplySorting(query, queryParams.Sort, queryParams.SortDirection);
        else if (queryParams.SortDirection == "asc")
            query = query.OrderBy(i => i.CreatedAt);
        else
            query = query.OrderByDescending(i => i.CreatedAt);

        var skip = (queryParams.Page - 1) * queryParams.Size;
        var tasks = await query
                            .Skip(skip)
                            .Take(queryParams.Size)
                            .ToListAsync();

        var taskDtos = _mapper.Map<IEnumerable<InvoiceResponseDto>>(tasks);

        return PagedResult<InvoiceResponseDto>.Create(
            taskDtos,
            queryParams.Page,
            queryParams.Size,
            totalCount
            );

    }

    public IQueryable<Invoice> ApplySorting(
        IQueryable<Invoice> query,
        string sort,
        string? sortDirection)
    {
        bool isDescending = sortDirection?.ToLower() == "desc";

        return sort.ToLower() switch
        {
            "startdate" => isDescending
                ? query.OrderByDescending(i => i.StartDate)
                : query.OrderBy(i => i.StartDate),
            "enddate" => isDescending
                ? query.OrderByDescending(i => i.EndDate)
                : query.OrderBy(i => i.EndDate),
            "status" => isDescending
                ? query.OrderByDescending(i => i.Status)
                : query.OrderBy(i => i.Status),
            "createdat" => isDescending
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt),
            "updatedat" => isDescending
                ? query.OrderByDescending(i => i.UpdatedAt)
                : query.OrderBy(i => i.UpdatedAt),
            _ => query.OrderBy(i => i.CreatedAt)
        };
    }
}
