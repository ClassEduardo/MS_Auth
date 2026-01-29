using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MS_Auth.Domain.Models;
using MS_Auth.Infrastructure.Auth.Interface;

namespace MS_Auth.Infrastructure.Auth.Service;

public class GerarToken(
    IConfiguration configuration
) : IGerarToken
{
    private readonly IConfiguration _configuration = configuration;

    public Token Gerar(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = expiresAt,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new Token
        {
            Id = Guid.NewGuid(),
            UserToken = tokenHandler.WriteToken(token),
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            UserId = user.Id,
            ExpiresAt = expiresAt
        };
    }
}