using MS_Auth.Application.DTOs.Request;
using MS_Auth.Domain.DTOs.Request;
using MS_Auth.Domain.DTOs.Response;
using MS_Auth.Domain.Models;

namespace MS_Auth.Infrastructure.Auth.Interface;

public interface IAuthService
{
    Task<AuthResponse<TokenResponse>?> LoginAsync(LoginRequest request);
    Task<AuthResponse<TokenResponse>?> RegisterAsync(RegisterRequest request);
}