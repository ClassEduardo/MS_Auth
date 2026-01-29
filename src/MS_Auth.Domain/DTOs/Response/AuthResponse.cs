namespace MS_Auth.Domain.DTOs.Response;

public class AuthResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}