namespace ASP_NET_Final_Proj.DTOs.CustomerDTOs;

/// <summary>
/// Data Transfer Object for creating a new customer
/// Used in HTTP POST requests to add a customer to the system
/// </summary>
public class CreateCustomerDto
{
    /// <summary>
    /// Full name of the customer
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; }

    /// <summary>
    /// Email address of the customer
    /// Should be unique and valid
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; }

    /// <summary>
    /// Physical address of the customer
    /// Can include street, city, and country
    /// </summary>
    /// <example>123 Main Street, Baku, Azerbaijan</example>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Contact phone number of the customer
    /// Include country code if applicable
    /// </summary>
    /// <example>+994501234567</example>
    public string PhoneNumber { get; set; } = string.Empty;
}

