using ASP_NET_Final_Proj.Models;

namespace ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;

/// <summary>
/// Data Transfer Object representing an invoice row in responses
/// Returned by GET, POST, and PUT endpoints
/// </summary>
public class InvoiceRowResponseDto
{
    /// <summary>
    /// Unique identifier of the invoice row
    /// </summary>
    /// <example>5fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the invoice this row belongs to
    /// </summary>
    /// <example>4fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// Name of the service or product
    /// </summary>
    /// <example>Web Development Service</example>
    public string Service { get; set; }

    /// <summary>
    /// Quantity of the service/product
    /// </summary>
    /// <example>2</example>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit price of the service/product
    /// </summary>
    /// <example>500.50</example>
    public decimal Amount { get; set; }

    /// <summary>
    /// Total sum for this row (Quantity * Amount)
    /// </summary>
    /// <example>1001.00</example>
    public decimal Sum { get; set; }
}

