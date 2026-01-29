

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MS_Auth.Application.DTOs.Request;
using MS_Auth.Domain.DTOs.Request;
using MS_Auth.Domain.DTOs.Response;
using MS_Auth.Domain.Models;
using MS_Auth.Domain.Repositories;
using MS_Auth.Infrastructure.Auth.Interface;
using Newtonsoft.Json;

namespace MS_Auth.Infrastructure.Auth.Service;

public class AuthService(
    IGerarToken generateToken,
    IRepositoryBase<User> userRepositoryBase,
    IRepositoryBase<Token> tokenRepositoryBase
) : IAuthService
{
    private readonly IGerarToken _generateToken = generateToken;
    private readonly IRepositoryBase<User> _userRepositoryBase = userRepositoryBase;
    private readonly IRepositoryBase<Token> _tokenRepositoryBase = tokenRepositoryBase;

    public async Task<AuthResponse<TokenResponse>?> RegisterAsync(RegisterRequest request)
    {
        var erro = await Validacoes(request);

        if (erro != null)
        {
            return new AuthResponse<TokenResponse>
            {
                Success = false,
                Message = erro,
                Data = null
            };
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = HashPassword(request.Password),
            Role = request.Role
        };

        await _userRepositoryBase.CriarAsync(newUser);
        var response = _generateToken.Gerar(newUser);

        await _tokenRepositoryBase.CriarAsync(response);

        return new AuthResponse<TokenResponse>
        {
            Success = true,
            Message = "Usuário criado.",
            Data = new TokenResponse
            {
                AccessToken = response.UserToken,
                ExpiresAt = response.ExpiresAt
            }
        };
    }

    public async Task<AuthResponse<TokenResponse>?> LoginAsync(LoginRequest request)
    {
        User user = await _userRepositoryBase.ObterPorFiltroAsync(u => u.Email == request.Email);

        if (user == null || !VerifyPassword(request.Password, user.Password))
        {
            return null;
        }

        var response = _generateToken.Gerar(user);

        return await Task.FromResult(new AuthResponse<TokenResponse>
        {
            Success = true,
            Message = "Usuário valido.",
            Data = new TokenResponse
            {
                AccessToken = response.UserToken,
                ExpiresAt = response.ExpiresAt,
            }
        });
    }

    private async Task<string?> Validacoes(RegisterRequest request)
    {
        var user = await _userRepositoryBase.ObterPorFiltroAsync(u => u.Email == request.Email);

        if (user != null) return "Usuário Já existe";
        if (string.IsNullOrEmpty(request.Name)) return "Nome Inválido";
        if (string.IsNullOrEmpty(request.Email)) return "Email INnválido";
        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
            return "Senha inválida, ter no mínimo 8 caracteres.";

        if (request.Role != "Administrador" || request.Role != "Vendedor")
        {
            return "Perfil precisa ser \"Administrador\" ou \"Vendedor\"";
        }

        return null;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hashedPassword;
    }
}