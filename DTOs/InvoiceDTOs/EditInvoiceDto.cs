using ASP_NET_Final_Proj.Models;

namespace ASP_NET_Final_Proj.DTOs.InvoiceDTOs;

/// <summary>
/// Data Transfer Object for editing an existing invoice
/// Used in HTTP PUT requests
/// </summary>
public class EditInvoiceDto
{
    /// <summary>
    /// Updated start date of the invoice
    /// </summary>
    /// <example>2026-01-01T09:00:00+00:00</example>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// Updated end date of the invoice
    /// </summary>
    /// <example>2026-01-31T18:00:00+00:00</example>
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Updated comment or note about the invoice
    /// </summary>
    /// <example>Payment updated to include late fees</example>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Updated status of the invoice
    /// </summary>
    /// <example>Paid</example>
    public InvoiceStatus Status { get; set; }
}
