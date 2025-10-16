using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AleStock.Api.Data;
using AleStock.Api.Models;
using AleStock.Api.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AleStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AleStockDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthController(AleStockDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest request)
    {
        var user = _context.Usuarios
            .FirstOrDefault(u => u.Email == request.Email && u.PasswordHash == request.Password);

        if (user == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            }),
            Expires = DateTime.UtcNow.AddHours(4),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return Ok(new AuthResponse
        {
            Token = jwt,
            Nombre = user.Nombre,
            Rol = user.Rol
        });
    }
}
