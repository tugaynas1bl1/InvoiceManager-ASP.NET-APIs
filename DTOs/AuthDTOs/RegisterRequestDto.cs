namespace ASP_NET_Final_Proj.DTOs.AuthDTOs;

public class RegisterRequestDto
{
    /// <summary>
    /// Full name of the user.
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; }

    /// <summary>
    /// Optional address of the user.
    /// </summary>
    /// <example>123 Main Street</example>
    public string? Address { get; set; }

    /// <summary>
    /// Optional phone number of the user.
    /// </summary>
    /// <example>+1234567890</example>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    /// <example>Password123</example>
    public string Password { get; set; }

    /// <summary>
    /// Confirmation of the user's password. Must match Password.
    /// </summary>
    /// <example>Password123</example>
    public string ConfirmPassword { get; set; }
}
