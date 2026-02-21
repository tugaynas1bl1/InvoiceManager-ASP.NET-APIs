namespace ASP_NET_Final_Proj.DTOs.AuthDTOs;

public class RefreshTokenRequest
{
    /// <summary>
    /// The refresh token provided to obtain a new access token.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string RefreshToken { get; set; } = string.Empty;
}
