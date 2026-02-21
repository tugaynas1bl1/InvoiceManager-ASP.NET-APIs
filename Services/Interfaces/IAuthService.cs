using ASP_NET_Final_Proj.DTOs.AuthDTOs;

namespace ASP_NET_Final_Proj.Services.Interfaces;

public interface IAuthService
{
    Task <AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task <AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task <AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshToken);
    Task <UserResponseDto> EditProfileAsync(EditProfileRequestDto edittedRequest);
    Task SendVerificationCodeAsync();
    Task <bool> ChangePasswordAsync(ChangePasswordRequest changedPassword);
    Task <bool> DeleteProfileAsync(DeleteRequestDto deleteRequest);
}
