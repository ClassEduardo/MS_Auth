using MS_Auth.Domain.Models;

namespace MS_Auth.Infrastructure.Auth.Interface;

public interface IGerarToken
{
    Token Gerar(User user);
}