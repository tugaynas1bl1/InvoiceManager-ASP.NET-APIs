namespace ASP_NET_Final_Proj.DTOs.AuthDTOs;

public class ChangePasswordRequest
{
    /// <summary>
    /// Verification code sent to the user for password change confirmation.
    /// </summary>
    /// <example>123456</example>
    public int Code { get; set; }

    /// <summary>
    /// The new password the user wants to set.
    /// </summary>
    /// <example>NewSecurePassword123</example>
    public string NewPassword { get; set; }
}
