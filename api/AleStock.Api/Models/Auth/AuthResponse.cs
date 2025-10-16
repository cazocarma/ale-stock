namespace AleStock.Api.Models.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
