using ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_Final_Proj.Services.Classes;

public class InvoiceRowService : IInvoiceRowService
{
    private readonly InvoiceManagerDbContext _context;
    private readonly IMapper _mapper;

    public InvoiceRowService(InvoiceManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InvoiceRowResponseDto> CreateAsync(CreateInvoiceRowDto createdRowRequest)
    {
        var row = _mapper.Map<InvoiceRow>(createdRowRequest);

        row.Sum = row.Amount * row.Quantity;

        _context.InvoiceRows.Add(row);
        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceRowResponseDto>(row);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var row = await _context
            .InvoiceRows
            .Include(ir => ir.Invoice)
            .FirstAsync(c => c.Id == id);

        if (row is null) return false;

        _context.InvoiceRows.Remove(row);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<InvoiceRowResponseDto> EditAsync(Guid id, EditInvoiceRowDto edittedRowRequest)
    {
        var edittedRow = await _context
            .InvoiceRows
            .Include(ir => ir.Invoice)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (edittedRow is null) return null;

        _mapper.Map(edittedRowRequest, edittedRow);
        edittedRow.Sum = edittedRow.Amount * edittedRow.Quantity;

        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceRowResponseDto>(edittedRow);
    }

    public async Task<IEnumerable<InvoiceRowResponseDto>> GetAllAsync()
    {
        var rows = await _context
            .InvoiceRows
            .Include(ir => ir.Invoice)
            .ToListAsync();

        return rows.Select(c => _mapper.Map<InvoiceRowResponseDto>(c));
    }

    public async Task<InvoiceRowResponseDto> GetByIdAsync(Guid id)
    {
        var row = await _context
            .InvoiceRows
            .Include(ir => ir.Invoice)
            .FirstOrDefaultAsync(c => c.Id == id);

        return _mapper.Map<InvoiceRowResponseDto>(row);
    }
}
