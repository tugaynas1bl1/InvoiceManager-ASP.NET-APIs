namespace ASP_NET_Final_Proj.DTOs.AuthDTOs;

public class EditProfileRequestDto
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
    /// <example>456 Another Street</example>
    public string? Address { get; set; }

    /// <summary>
    /// Optional phone number of the user.
    /// </summary>
    /// <example>+9876543210</example>
    public string? PhoneNumber { get; set; }
}
