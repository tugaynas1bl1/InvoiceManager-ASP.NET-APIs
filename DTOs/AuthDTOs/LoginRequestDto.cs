namespace ASP_NET_Final_Proj.DTOs.AuthDTOs;

public class LoginRequestDto
{
    /// <summary>
    /// User's email address used for authentication.
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; }

    /// <summary>
    /// User's password used for authentication.
    /// </summary>
    /// <example>Password123</example>
    public string Password { get; set; }
}
