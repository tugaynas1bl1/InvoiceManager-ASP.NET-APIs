namespace ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;

/// <summary>
/// Data Transfer Object for creating a new invoice row
/// Used in HTTP POST requests
/// </summary>
public class CreateInvoiceRowDto
{
    /// <summary>
    /// ID of the invoice this row belongs to
    /// </summary>
    /// <example>4fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// Name of the service or product in this invoice row
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
}
