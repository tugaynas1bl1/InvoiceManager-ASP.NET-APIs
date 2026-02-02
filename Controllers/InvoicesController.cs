using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.DTOs.InvoiceDTOs;
using ASP_NET_Final_Proj.DTOs.QueryDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_Final_Proj.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    /// <summary>
    /// Adds a new invoice
    /// </summary>
    /// <param name="createdInvoiceRequest">The payload used to add a new invoice</param>
    /// <returns>The created invoice wrapped in InvoiceResponseDto</returns>
    /// <response code="201">The invoice was successfully added</response>
    /// <response code="400">The request body is invalid</response>
    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> AddInvoice([FromBody] CreateInvoiceDto createdInvoiceRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdInvoice = await _invoiceService.CreateAsync(createdInvoiceRequest);

        return CreatedAtAction(nameof(GetInvoiceById), new { id = createdInvoice.Id }, createdInvoice);
    }

    /// <summary>
    /// Archives (soft deletes) an invoice by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the invoice</param>
    /// <returns>No content if archived successfully</returns>
    /// <response code="204">The invoice was successfully archived</response>
    /// <response code="404">Invoice not found</response>
    [HttpPatch("archive/{id}")]
    public async Task<ActionResult<bool>> ArchiveInvoice(Guid id)
    {
        var isArchived = await _invoiceService.ArchiveAsync(id);

        if (!isArchived)
            return NotFound($"Invoice with ID {id} NOT FOUND");

        return NoContent();
    }

    /// <summary>
    /// Permanently deletes an invoice by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the invoice</param>
    /// <returns>No content if deleted successfully</returns>
    /// <response code="204">The invoice was successfully deleted</response>
    /// <response code="404">Invoice not found</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteInvoice(Guid id)
    {
        var isDeleted = await _invoiceService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound($"Invoice with ID {id} NOT FOUND");

        return NoContent();
    }

    /// <summary>
    /// Updates an existing invoice by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the invoice</param>
    /// <param name="edittedInvoiceRequest">The payload containing updated invoice data</param>
    /// <returns>The updated invoice wrapped in InvoiceResponseDto</returns>
    /// <response code="200">The invoice was successfully updated</response>
    /// <response code="400">The request body is invalid</response>
    /// <response code="404">Invoice not found</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> EditInvoice(Guid id, EditInvoiceDto edittedInvoiceRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var edittedInvoice = await _invoiceService.EditAsync(id, edittedInvoiceRequest);

        if (edittedInvoice is null)
            return NotFound($"Invoice with ID {id} NOT FOUND");

        return Ok(edittedInvoice);
    }

    /// <summary>
    /// Retrieves all invoices
    /// </summary>
    /// <returns>A list of invoices</returns>
    /// <response code="200">Invoices retrieved successfully</response>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetAllInvoices()
    {
        var invoices = await _invoiceService.GetAllAsync();
        if (invoices is null) return BadRequest("No any invoices");
        return Ok(invoices);
    }

    /// <summary>
    /// Retrieves an invoice by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the invoice</param>
    /// <returns>The invoice wrapped in InvoiceResponseDto</returns>
    /// <response code="200">Invoice retrieved successfully</response>
    /// <response code="404">Invoice not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetInvoiceById(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);

        if (invoice is null)
            return NotFound($"Invoice with ID {id} NOT FOUND");

        return Ok(invoice);
    }

    /// <summary>
    /// Changes the status of an invoice
    /// </summary>
    /// <param name="id">The unique identifier of the invoice</param>
    /// <param name="status">The new status to set</param>
    /// <returns>The updated invoice wrapped in InvoiceResponseDto</returns>
    /// <response code="200">Invoice status updated successfully</response>
    /// <response code="404">Invoice not found</response>
    [HttpPatch("changestatus/{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> ChangeInvoiceStatus(Guid id, InvoiceStatus status)
    {
        var invoice = await _invoiceService.ChangeStatusAsync(id, status);

        if (invoice is null)
            return NotFound($"Invoice with ID {id} NOT FOUND");

        return Ok(invoice);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<InvoiceResponseDto>>> GetPaged([FromQuery] InvoiceQueryParams queryParams)
    {
        var result = await _invoiceService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<InvoiceResponseDto>>.SuccessResponse(result));
    }
}
