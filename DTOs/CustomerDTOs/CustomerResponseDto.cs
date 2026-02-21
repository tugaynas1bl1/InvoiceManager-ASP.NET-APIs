namespace ASP_NET_Final_Proj.DTOs.CustomerDTOs;

/// <summary>
/// Data Transfer Object representing a customer in responses
/// Returned by GET, POST, and PUT endpoints
/// </summary>
public class CustomerResponseDto
{
    /// <summary>
    /// Unique identifier of the customer
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique identifier of the customer
    /// </summary>
    /// <example>tugaynasibli@gmail.com (Tugay Nasibli)</example>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Full name of the customer
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; }

    /// <summary>
    /// Email address of the customer
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; }

    /// <summary>
    /// Physical address of the customer
    /// </summary>
    /// <example>123 Main Street, Baku, Azerbaijan</example>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Contact phone number of the customer
    /// </summary>
    /// <example>+994501234567</example>
    public string PhoneNumber { get; set; } = string.Empty;
}

