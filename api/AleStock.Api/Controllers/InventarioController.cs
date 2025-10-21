using AleStock.Api.Data;
using AleStock.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AleStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Bodega")]
public class InventarioController : ControllerBase
{
    private readonly AleStockDbContext _context;

    public InventarioController(AleStockDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Obtener inventario completo
    [HttpGet]
    public async Task<IActionResult> ObtenerInventario()
    {
        var inventario = await _context.Inventarios
            .Include(i => i.Producto)
            .OrderBy(i => i.Producto == null ? string.Empty : (i.Producto.Sku ?? string.Empty))
            .ToListAsync();

        return Ok(inventario);
    }

    // 2️⃣ Ajustar stock manualmente (por daño o ingreso)
    [HttpPost("ajustar")]
    public async Task<IActionResult> AjustarStock([FromBody] Movimiento ajuste)
    {
        var inventario = await _context.Inventarios
            .FirstOrDefaultAsync(i => i.ProductoId == ajuste.ProductoId);

        if (inventario == null)
            return NotFound($"No existe inventario para el producto {ajuste.ProductoId}");

        inventario.Cantidad += ajuste.Cantidad; // puede ser negativo o positivo

        var movimiento = new Movimiento
        {
            ProductoId = ajuste.ProductoId,
            Tipo = ajuste.Cantidad > 0 ? "ENTRADA" : "AJUSTE",
            Cantidad = ajuste.Cantidad,
            Comentario = ajuste.Comentario ?? "Ajuste manual de inventario",
            UsuarioId = ajuste.UsuarioId,
            Fecha = DateTime.UtcNow
        };

        _context.Movimientos.Add(movimiento);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Stock ajustado correctamente",
            inventario,
            movimiento
        });
    }
}
