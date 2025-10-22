using AleStock.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AleStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Bodega")]
public class MovimientosController : ControllerBase
{
    private readonly AleStockDbContext _context;

    public MovimientosController(AleStockDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Listar todos los movimientos
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var movimientos = await _context.Movimientos
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

        return Ok(movimientos);
    }

    // 2️⃣ Obtener movimientos por producto
    [HttpGet("producto/{productoId}")]
    public async Task<IActionResult> GetByProducto(int productoId)
    {
        var movimientos = await _context.Movimientos
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .Where(m => m.ProductoId == productoId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

        return Ok(movimientos);
    }
}
