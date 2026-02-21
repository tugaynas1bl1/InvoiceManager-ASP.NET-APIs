using ASP_NET_Final_Proj.Config;
using ASP_NET_Final_Proj.Data;
using ASP_NET_Final_Proj.DTOs.AuthDTOs;
using ASP_NET_Final_Proj.Models;
using ASP_NET_Final_Proj.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP_NET_Final_Proj.Services.Classes;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly InvoiceManagerDbContext _context;
    private readonly JwtConfig _jwtConfig;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;

    private const string RefreshTokenType = "refresh";

    public AuthService(UserManager<User> userManager, IConfiguration configuration, InvoiceManagerDbContext context, IOptions<JwtConfig> jwtConfig, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
        _jwtConfig = jwtConfig.Value;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
    }

    public async Task SendVerificationCodeAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId!);

        if (user is null)
            return;
        Random random = new Random();
        user.CodeVerification = random.Next(100000, 1000000);
        user.CodeVerificationExpiresAt = DateTime.UtcNow.AddMinutes(3);

        await _context.SaveChangesAsync();

        _emailService?.SendEmailAsync(user.Email!, user.CodeVerification);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest changedPassword)
    {
        var userId = _httpContextAccessor?.HttpContext?
            .User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        if (user.CodeVerification == default)
            return false;

        if (DateTimeOffset.UtcNow >= user.CodeVerificationExpiresAt)
        {
            user.CodeVerification = default;
            user.CodeVerificationExpiresAt = null;

            await _context.SaveChangesAsync();
            return false;
        }
            

        if (changedPassword.Code != user.CodeVerification)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            changedPassword.NewPassword
        );

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(x => x.Description));
            throw new Exception(errors);
        }

        user.CodeVerification = default;
        user.CodeVerificationExpiresAt = null;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProfileAsync(DeleteRequestDto deleteRequest)
    {
        if (deleteRequest.DeleteOrNot.ToLower().Trim() == "no")
            return false;

        else if (deleteRequest.DeleteOrNot.ToLower().Trim() == "yes")
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId!);

            if (user is null)
                return false;

            var refreshTokensWithThisUser = _context.RefreshTokens.Where(x => x.UserId == userId);

            if (refreshTokensWithThisUser is not null)
            {
                _context.RefreshTokens.RemoveRange(refreshTokensWithThisUser);
                await _context.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return false;

            return true;
        }

        else
            return false;
    }

    public async Task<UserResponseDto> EditProfileAsync(EditProfileRequestDto edittedRequest)
    {
        var emailUser = await _userManager.FindByEmailAsync(edittedRequest.Email);

        if (emailUser is not null)
            throw new InvalidOperationException("This email already exists! Use another email.");

        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new InvalidOperationException("Invalid user");

        var user = await _userManager.FindByIdAsync(userId!);

        if (user is null)
            throw new InvalidOperationException("Invalid user");

        user.Email = edittedRequest.Email;
        user.UserName = edittedRequest.Email;
        user.Name = edittedRequest.Name;
        user.PhoneNumber = edittedRequest.PhoneNumber;
        user.Address = edittedRequest.Address;

        var result = await _userManager.UpdateAsync(user);


        if (!result.Succeeded)
            throw new InvalidOperationException("Invalid user");

        return new UserResponseDto
        {
            Email = user.Email,
            Name = user.Name,
            Address = user.Address!,
            PhoneNumber = user.PhoneNumber!
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (!isValidPassword)
            throw new UnauthorizedAccessException("Invalid email or password");

        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var (principal, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken);
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);

        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId!);    

        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        storedToken.RevokedAt = DateTime.UtcNow;

        var newTokens = await GenerateTokenAsync(user);

        var newStoredToken = await _context
                                .RefreshTokens
                                .FirstOrDefaultAsync(rt => rt.JwtId == GetJtiFromRefreshToken(newTokens.RefreshToken));

        if (newStoredToken is not null)
            storedToken.ReplaceByJwtId = newStoredToken.JwtId;

        await _context.SaveChangesAsync();
        return newTokens;
    }

    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(refreshJwt)) return string.Empty;

        var jwt = handler.ReadJwtToken(refreshJwt);
        
        return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }    

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        var user = await _userManager.FindByEmailAsync(registerRequest.Email);

        if (user is not null)
            throw new InvalidOperationException("User with this email already exists");

        User newUser = new User
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            Name = registerRequest.Name,
            PhoneNumber = registerRequest.PhoneNumber,
            Address = registerRequest.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var result = await _userManager.CreateAsync(newUser, registerRequest.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        return await GenerateTokenAsync(newUser!);
    }

    private (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifeTime = true)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey!));

        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = validateLifeTime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer,
            ValidAudience = _jwtConfig.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;

        if (tokenType is not RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid refresh token");


        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        return (principal, jti);
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequest refreshToken)
    {
        string? jti;
        try
        {
            (_, jti) = ValidateRefreshJwtAndGetJti(refreshToken.RefreshToken, validateLifeTime: false);
        }
        catch
        {
            return;
        }

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);

        if (storedToken is null || !storedToken.IsActive)
            return;

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var (refreshToken, refreshJwt) = await CreateRefreshTokenJwtAsync(user.Id, _jwtConfig.RefreshTokenExpirationDays);

        return new AuthResponseDto
        {
            AccessToken = tokenString,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            RefreshToken = refreshJwt,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            Email = user.Email,
        };
    }

    private async Task<(RefreshToken refreshToken, string refreshJwt)> CreateRefreshTokenJwtAsync(string userId, int expirationDays)
    {
        var jti = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim("token_type", RefreshTokenType)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync();

        return (refreshToken, jwtString);
    }
}
