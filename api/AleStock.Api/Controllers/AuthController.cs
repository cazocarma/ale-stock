using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AleStock.Api.Data;
using AleStock.Api.Helpers;
using AleStock.Api.Models;
using AleStock.Api.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Email y contraseña son requeridos" });

        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Credenciales inválidas" });

        // 🔹 Claims personalizados con nombres simples
        var claims = new[]
        {
            new Claim("email", user.Email),
            new Claim("role", user.Rol),
            new Claim("userId", user.Id.ToString()),
            new Claim("nombre", user.Nombre ?? string.Empty)
        };

        // 🔹 Generar el token JWT
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new AuthResponse
        {
            Token = jwt,
            Nombre = user.Nombre ?? string.Empty,
            Rol = user.Rol
        });
    }

}
