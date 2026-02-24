using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.CustomerDTOs;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Drawing;
using System.Security.Claims;
using Xceed.Document.NET;
using Xceed.Words.NET;

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
            .FirstOrDefaultAsync(i => i.Id == id && i.Customer.UserId == userId && i.Status == InvoiceStatus.Created);

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

    public async Task<byte[]> DownloadPdfAsync(Guid id)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoice = await _context
                            .Invoices
                            .Include(i => i.Customer)
                            .Where(i => i.DeletedAt == null && i.Customer.UserId == userId)
                            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null)
            return null!;

        using var ms = new MemoryStream();

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.Background(Colors.Grey.Lighten3);
                page.Content().Column(col =>
                {
                    // Title
                    col.Item().Text("INVOICE")
                        .FontSize(32)
                        .Bold()
                        .FontColor(Colors.Blue.Medium)
                        .AlignCenter();

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Darken2);

                    // Main Info Section
                    col.Item().Column(info =>
                    {
                        info.Item().Text($"Invoice Id: {invoice.Id}")
                            .FontSize(14)
                            .SemiBold()
                            .FontColor(Colors.Black);

                        info.Item().Text($"Customer: {invoice.Customer.Name}")
                            .FontSize(14);

                        info.Item().Text($"Total Sum: {invoice.TotalSum:C}")
                            .FontSize(14)
                            .FontColor(Colors.Green.Darken1)
                            .Bold();

                        info.Item().Text($"Comment: {invoice.Comment}")
                            .FontSize(12)
                            .Italic();
                    });

                    col.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Darken1);

                    // Dates & Status Section
                    col.Item().Column(dates =>
                    {
                        dates.Item().Text($"Status: {invoice.Status}")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Orange.Darken1);

                        dates.Item().Text($"Created At: {invoice.CreatedAt:dd.MM.yyyy}")
                            .FontSize(12);

                        dates.Item().Text($"Start Date: {invoice.StartDate:dd.MM.yyyy}")
                            .FontSize(12);

                        dates.Item().Text($"End Date: {invoice.EndDate:dd.MM.yyyy}")
                            .FontSize(12);
                    });

                    col.Item().PaddingTop(20)
                        .Text("Thank you for your business!")
                        .FontSize(14)
                        .Italic()
                        .FontColor(Colors.Grey.Darken2)
                        .AlignCenter();
                });
            });
        });

        document.GeneratePdf(ms);

        return ms.ToArray();
    }
    public async Task<byte[]> DownloadDocxAsync(Guid id) 
    {
        var userId = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        var invoice = await _context
                            .Invoices
                            .Include(i => i.Customer)
                            .Where(i => i.DeletedAt == null && i.Customer.UserId == userId)
                            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null)
            return null!;

        using var ms = new MemoryStream();

        using (var doc = DocX.Create(ms))
        {
            // Title
            var title = doc.InsertParagraph("INVOICE")
                           .FontSize(28)
                           .Bold()
                           .Color(Xceed.Drawing.Color.DarkBlue);
            title.Alignment = Alignment.center;

            doc.InsertParagraph(Environment.NewLine); // spacing

            // Invoice Info Section
            var infoTable = doc.AddTable(4, 2);
            infoTable.Design = TableDesign.ColorfulListAccent1;
            infoTable.AutoFit = AutoFit.Contents;
            infoTable.Alignment = Alignment.center; // <-- Center the table

            infoTable.Rows[0].Cells[0].Paragraphs[0].Append("Invoice Id").Bold();
            infoTable.Rows[0].Cells[1].Paragraphs[0].Append(invoice.Id.ToString());

            infoTable.Rows[1].Cells[0].Paragraphs[0].Append("Customer").Bold();
            infoTable.Rows[1].Cells[1].Paragraphs[0].Append(invoice.Customer.Name);

            infoTable.Rows[2].Cells[0].Paragraphs[0].Append("Created At").Bold();
            infoTable.Rows[2].Cells[1].Paragraphs[0].Append(invoice.CreatedAt.ToString("yyyy-MM-dd"));

            infoTable.Rows[3].Cells[0].Paragraphs[0].Append("Status").Bold();
            infoTable.Rows[3].Cells[1].Paragraphs[0].Append(invoice.Status.ToString());

            doc.InsertTable(infoTable);

            doc.InsertParagraph(Environment.NewLine); // spacing

            // Invoice Details Section
            var detailsTable = doc.AddTable(4, 2);
            detailsTable.Design = TableDesign.MediumList2Accent2;
            detailsTable.AutoFit = AutoFit.Contents;
            detailsTable.Alignment = Alignment.center; // <-- Center the table

            detailsTable.Rows[0].Cells[0].Paragraphs[0].Append("Start Date").Bold();
            detailsTable.Rows[0].Cells[1].Paragraphs[0].Append(invoice.StartDate.ToString("yyyy-MM-dd"));

            detailsTable.Rows[1].Cells[0].Paragraphs[0].Append("End Date").Bold();
            detailsTable.Rows[1].Cells[1].Paragraphs[0].Append(invoice.EndDate.ToString("yyyy-MM-dd"));

            detailsTable.Rows[2].Cells[0].Paragraphs[0].Append("Total Sum").Bold().Color(Xceed.Drawing.Color.DarkGreen);
            detailsTable.Rows[2].Cells[1].Paragraphs[0].Append(invoice.TotalSum.ToString("C")).Color(Xceed.Drawing.Color.DarkGreen);

            detailsTable.Rows[3].Cells[0].Paragraphs[0].Append("Comment").Bold();
            detailsTable.Rows[3].Cells[1].Paragraphs[0].Append(invoice.Comment);

            doc.InsertTable(detailsTable);

            doc.InsertParagraph(Environment.NewLine); // spacing

            // Footer
            var footer = doc.InsertParagraph("Thank you for your business!")
                            .FontSize(14)
                            .Italic()
                            .Color(Xceed.Drawing.Color.Gray);
            footer.Alignment = Alignment.center;

            doc.Save();
        }

        return ms.ToArray();
    }
}
