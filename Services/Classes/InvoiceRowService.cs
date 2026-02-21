using ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_NET_Final_Proj.Services.Classes;

public class InvoiceRowService : IInvoiceRowService
{
    private readonly InvoiceManagerDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InvoiceRowService(InvoiceManagerDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<InvoiceRowResponseDto> CreateAsync(CreateInvoiceRowDto createdRowRequest)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoice = await _context
            .Invoices
            .Include(i => i.Rows)
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i =>
                i.Id == createdRowRequest.InvoiceId &&
                i.DeletedAt == null &&
                i.Customer.UserId == userId);

        var row = _mapper.Map<InvoiceRow>(createdRowRequest);
        row.Sum = row.Amount * row.Quantity;

        if (invoice != null)
        {
            invoice.Rows.Add(row);
            invoice.TotalSum = invoice.Rows.Sum(r => r.Sum);
        }
        else 
           throw new NullReferenceException();

        await _context.SaveChangesAsync();   

        return _mapper.Map<InvoiceRowResponseDto>(row);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var row = await _context
            .InvoiceRows
            .Include(r => r.Invoice)
                .ThenInclude(i => i.Customer)
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.Invoice.Customer.UserId == userId);

        if (row is null) 
            return false;

        var invoice = await _context
            .Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == row.InvoiceId);

        if (invoice is not null)
        {
            invoice.Rows.Remove(row);
            invoice.TotalSum = invoice.Rows.Sum(r => r.Sum);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<InvoiceRowResponseDto> EditAsync(Guid id, EditInvoiceRowDto edittedRowRequest)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var edittedRow = await _context
            .InvoiceRows
            .Include(r => r.Invoice)
                .ThenInclude(i => i.Customer)
            .Include(r => r.Invoice.Rows)
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.Invoice.Customer.UserId == userId);

        if (edittedRow is null)
            return null;

        _mapper.Map(edittedRowRequest, edittedRow);
        edittedRow.Sum = edittedRow.Amount * edittedRow.Quantity;

        var invoice = edittedRow.Invoice;
        invoice.TotalSum = invoice.Rows.Sum(r => r.Sum);

        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceRowResponseDto>(edittedRow);
    }

    public async Task<IEnumerable<InvoiceRowResponseDto>> GetAllAsync()
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var rows = await _context
            .InvoiceRows
            .Include(r => r.Invoice)
                .ThenInclude(i => i.Customer)
            .Where(r => r.Invoice.Customer.UserId == userId)
            .ToListAsync();

        return rows.Select(r => _mapper.Map<InvoiceRowResponseDto>(r));
    }

    public async Task<InvoiceRowResponseDto> GetByIdAsync(Guid id)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var row = await _context
            .InvoiceRows
            .Include(r => r.Invoice)
                .ThenInclude(i => i.Customer)
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.Invoice.Customer.UserId == userId);

        return _mapper.Map<InvoiceRowResponseDto>(row);
    }
}
