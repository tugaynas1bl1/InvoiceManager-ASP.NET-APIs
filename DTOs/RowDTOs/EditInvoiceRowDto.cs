namespace ASP_NET_10._TaskFlow_Pagination_Filtering_Ordering.DTOs;

/// <summary>
/// Data Transfer Object for editing an existing invoice row
/// Used in HTTP PUT requests
/// </summary>
public class EditInvoiceRowDto
{
    /// <summary>
    /// Updated name of the service or product
    /// </summary>
    /// <example>Web Development Service - Premium</example>
    public string Service { get; set; }

    /// <summary>
    /// Updated quantity of the service/product
    /// </summary>
    /// <example>3</example>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Updated unit price of the service/product
    /// </summary>
    /// <example>550.75</example>
    public decimal Amount { get; set; }
}

