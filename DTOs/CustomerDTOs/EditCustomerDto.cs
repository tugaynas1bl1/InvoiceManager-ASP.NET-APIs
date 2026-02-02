namespace ASP_NET_Final_Proj.DTOs.CustomerDTOs;

/// <summary>
/// DTO for editing an existing customer
/// Used in PUT requests
/// </summary>
public class EditCustomerDto
{
    /// <summary>
    /// Customer Name
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; }

    /// <summary>
    /// Customer Email
    /// </summary>
    /// <example>john.doe@email.com</example>
    public string Email { get; set; }

    /// <summary>
    /// Customer Address
    /// </summary>
    /// <example>Baku, Azerbaijan</example>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Customer Phone Number
    /// </summary>
    /// <example>+994501234567</example>
    public string PhoneNumber { get; set; } = string.Empty;
}