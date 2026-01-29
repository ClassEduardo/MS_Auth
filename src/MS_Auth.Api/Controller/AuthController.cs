

using Microsoft.AspNetCore.Mvc;
using MS_Auth.Application.DTOs.Request;
using MS_Auth.Domain.DTOs.Request;
using MS_Auth.Infrastructure.Auth.Interface;

namespace MS_Auth.Api.Controller;

public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return Ok(result);
    }
}