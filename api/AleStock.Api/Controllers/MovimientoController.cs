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
    public async Task<IActionResult> ObtenerMovimientos()
    {
        var movimientos = await _context.Movimientos
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

        return Ok(movimientos);
    }
}
