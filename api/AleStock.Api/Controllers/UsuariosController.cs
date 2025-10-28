using AleStock.Api.Data;
using AleStock.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AleStock.Api.Helpers; 

namespace AleStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Coordinador")]
public class UsuariosController : ControllerBase
{
    private readonly AleStockDbContext _context;

    public UsuariosController(AleStockDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Listar usuarios
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _context.Usuarios
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Usuario usuario)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
            return BadRequest("El correo ya está registrado.");

        usuario.PasswordHash = PasswordHasher.Hash(usuario.PasswordHash); // ✅ Cifrar
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Usuario creado correctamente", usuario });
    }

    // ...
    [HttpPatch("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] Usuario cambios)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound("Usuario no encontrado.");

        if (!string.IsNullOrWhiteSpace(cambios.Rol))
            usuario.Rol = cambios.Rol;

        if (!string.IsNullOrWhiteSpace(cambios.PasswordHash))
            usuario.PasswordHash = PasswordHasher.Hash(cambios.PasswordHash); // ✅ Cifrar

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Usuario actualizado correctamente", usuario });
    }
}
