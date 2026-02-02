using ASP_NET_Final_Proj.Models;

namespace ASP_NET_Final_Proj.DTOs.InvoiceDTOs;

/// <summary>
/// Data Transfer Object for creating a new invoice
/// Used in HTTP POST requests
/// </summary>
public class CreateInvoiceDto
{
    /// <summary>
    /// The ID of the customer associated with this invoice
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Start date of the invoice period
    /// </summary>
    /// <example>2026-01-01T09:00:00+00:00</example>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// End date of the invoice period
    /// </summary>
    /// <example>2026-01-31T18:00:00+00:00</example>
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Optional comment or note about the invoice
    /// </summary>
    /// <example>Payment for January services</example>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Status of the invoice
    /// </summary>
    /// <example>Pending</example>
    public InvoiceStatus Status { get; set; }
}
