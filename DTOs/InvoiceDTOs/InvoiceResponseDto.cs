using ASP_NET_Final_Proj.Models;

namespace ASP_NET_Final_Proj.DTOs.InvoiceDTOs;

/// <summary>
/// Data Transfer Object representing an invoice in responses
/// Returned by GET, POST, and PUT endpoints
/// </summary>
public class InvoiceResponseDto
{
    /// <summary>
    /// Unique identifier of the invoice
    /// </summary>
    /// <example>4fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

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
    /// Total sum of the invoice in the system currency
    /// </summary>
    /// <example>1500.75</example>
    public decimal TotalSum { get; set; }

    /// <summary>
    /// Optional comment or note about the invoice
    /// </summary>
    /// <example>Payment for January services</example>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the invoice
    /// </summary>
    /// <example>Pending</example>
    public InvoiceStatus Status { get; set; }

    /// <summary>
    /// Full name of the customer associated with the invoice
    /// </summary>
    /// <example>John Doe</example>
    public string CustomerName { get; set; } = string.Empty;
}
