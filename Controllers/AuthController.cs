using ASP_NET_Final_Proj.Common;
using ASP_NET_Final_Proj.DTOs.AuthDTOs;
using ASP_NET_Final_Proj.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_Final_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(
            [FromBody] RegisterRequestDto registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully!"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
            [FromBody] LoginRequestDto loginRequest)
        {
            var result = await _authService.LoginAsync(loginRequest);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User logged in successfully!"));
        }

        [HttpPut("editprofile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> EditProfile (
            [FromBody] EditProfileRequestDto editRequest)
        {
            var result = await _authService.EditProfileAsync(editRequest);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result, "Profile editted successfully"));
        }

        [HttpDelete("deleteprofile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProfile(
            [FromBody] DeleteRequestDto deleteRequest)
        {
            var result = await _authService.DeleteProfileAsync(deleteRequest);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Profile deleted successfully"));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(
        [FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenRequest);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully"));
        }

        [HttpPost("revoke")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Revoke(
        [FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            await _authService.RevokeRefreshTokenAsync(refreshTokenRequest);
            return Ok(ApiResponse<object>.SuccessResponse("Token refreshed successfully"));
        }

        [HttpGet("sendverificationcodetoemail")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> SendVerification()
        {
            await _authService.SendVerificationCodeAsync();
            return Ok();
        }

        [HttpPut("changepassword")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(
            [FromBody] ChangePasswordRequest passwordRequest)
        {
            var result = await _authService.ChangePasswordAsync(passwordRequest);

            if (!result)
                return BadRequest("Verification code or something else is incorrect");

            return Ok(ApiResponse<object>.SuccessResponse(result, "Password changed successfully!"));
        }
    }
}
