namespace MS_Auth.Domain.Models;

public class Token : IEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserToken { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}