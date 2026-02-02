using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_Final_Proj.Services.Classes;

public class InvoiceService : IInvoiceService
{
    private readonly InvoiceManagerDbContext _context;
    private readonly IMapper _mapper;

    public InvoiceService(InvoiceManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto createdInvoiceRequest)
    {
        var invoice = _mapper.Map<Invoice>(createdInvoiceRequest);

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var invoice = await _context
            .Invoices
            .Include(c => c.Customer)
            .FirstAsync(c => c.Id == id);

        if (invoice is null || invoice.DeletedAt is not null) return false;

        invoice.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await _context
            .Invoices
            .Include(c => c.Customer)
            .FirstAsync(c => c.Id == id);

        if (invoice is null) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<InvoiceResponseDto> EditAsync(Guid id, EditInvoiceDto edittedInvoiceRequest)
    {
        var edittedInvoice = await _context
            .Invoices
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (edittedInvoice is null || edittedInvoice.DeletedAt is not null) return null;

        _mapper.Map(edittedInvoiceRequest, edittedInvoice);

        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceResponseDto>(edittedInvoice);
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllAsync()
    {
        var invoices = await _context
            .Invoices
            .Include(c => c.Customer)
            .Where(c => c.DeletedAt == null)
            .ToListAsync();

        return invoices.Select(c => _mapper.Map<InvoiceResponseDto>(c));
    }

    public async Task<InvoiceResponseDto> GetByIdAsync(Guid id)
    {
        var invoice = await _context
            .Invoices
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (invoice.DeletedAt is not null) return null;

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto> ChangeStatusAsync(Guid id, InvoiceStatus status)
    {
        var invoice = await _context
            .Invoices
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        

        invoice.Status = status;
        await _context.SaveChangesAsync();

        
        var item = invoice.DeletedAt != null ? null : _mapper.Map<InvoiceResponseDto>(invoice);

        return item;
    }

    public async Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(InvoiceQueryParams queryParams)
    {
        queryParams.Validate();

        var query = _context
            .Invoices
            .Include(i => i.Customer)
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
                            .Where(t => t.DeletedAt == null)
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
