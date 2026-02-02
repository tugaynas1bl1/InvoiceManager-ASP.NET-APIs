using ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_Final_Proj.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceRowsController : ControllerBase
{
    private readonly IInvoiceRowService _rowService;

    public InvoiceRowsController(IInvoiceRowService rowService)
    {
        _rowService = rowService;
    }

    /// <summary>
    /// Adds a new invoice row
    /// </summary>
    /// <param name="createdRowRequest">The payload used to add a new invoice row</param>
    /// <returns>The created invoice row wrapped in InvoiceRowResponseDto</returns>
    /// <response code="201">The invoice row was successfully added</response>
    /// <response code="400">The request body is invalid</response>
    [HttpPost]
    public async Task<ActionResult<InvoiceRowResponseDto>> AddInvoiceRow([FromBody] CreateInvoiceRowDto createdRowRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdInvoiceRow = await _rowService.CreateAsync(createdRowRequest);

        return CreatedAtAction(nameof(GetInvoiceRowById), new { id = createdInvoiceRow.Id }, createdInvoiceRow);
    }

    /// <summary>
    /// Permanently deletes an invoice row by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the invoice row</param>
    /// <returns>No content if deleted successfully</returns>
    /// <response code="204">The invoice row was successfully deleted</response>
    /// <response code="404">Invoice row not found</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteInvoiceRow(Guid id)
    {
        var isDeleted = await _rowService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound($"Invoice row with ID {id} NOT FOUND");

        return NoContent();
    }

    /// <summary>
    /// Updates an existing invoice row by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the invoice row</param>
    /// <param name="edittedRowRequest">The payload containing updated invoice row data</param>
    /// <returns>The updated invoice row wrapped in InvoiceRowResponseDto</returns>
    /// <response code="200">The invoice row was successfully updated</response>
    /// <response code="400">The request body is invalid</response>
    /// <response code="404">Invoice row not found</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<InvoiceRowResponseDto>> EditInvoiceRow(Guid id, EditInvoiceRowDto edittedRowRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var edittedInvoiceRow = await _rowService.EditAsync(id, edittedRowRequest);

        if (edittedInvoiceRow is null)
            return NotFound($"InvoiceRow with ID {id} NOT FOUND");

        return Ok(edittedInvoiceRow);
    }

    /// <summary>
    /// Retrieves all invoice rows
    /// </summary>
    /// <returns>A list of invoice rows</returns>
    /// <response code="200">Invoice rows retrieved successfully</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceRowResponseDto>>> GetAllInvoiceRows()
    {
        var rows = await _rowService.GetAllAsync();
        if (rows is null) return BadRequest("No any invoice rows");
        return Ok(rows);
    }

    /// <summary>
    /// Retrieves an invoice row by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the invoice row</param>
    /// <returns>The invoice row wrapped in InvoiceRowResponseDto</returns>
    /// <response code="200">Invoice row retrieved successfully</response>
    /// <response code="404">Invoice row not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceRowResponseDto>> GetInvoiceRowById(Guid id)
    {
        var invoice = await _rowService.GetByIdAsync(id);

        if (invoice is null)
            return NotFound($"InvoiceRow with ID {id} NOT FOUND");

        return Ok(invoice);
    }
}
