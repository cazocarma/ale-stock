using AleStock.Api.Data;
using AleStock.Api.Models;
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

    // 1️⃣ Listar todos los movimientos (últimos primero)
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

    // 2️⃣ Filtrar por producto
    [HttpGet("producto/{productoId}")]
    public async Task<IActionResult> GetByProducto(int productoId)
    {
        var movimientos = await _context.Movimientos
            .Where(m => m.ProductoId == productoId)
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

        if (!movimientos.Any())
            return NotFound($"No existen movimientos para el producto {productoId}");

        return Ok(movimientos);
    }

    // 3️⃣ Filtrar por rango de fechas (se fuerza UTC)
    [HttpGet("rango")]
    public async Task<IActionResult> GetByFecha([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        // Forzamos las fechas a UTC para evitar error de Npgsql
        desde = DateTime.SpecifyKind(desde, DateTimeKind.Utc);
        hasta = DateTime.SpecifyKind(hasta, DateTimeKind.Utc);

        var movimientos = await _context.Movimientos
            .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

        return Ok(movimientos);
    }

    // 4️⃣ Resumen de entradas/salidas por producto
    [HttpGet("resumen/{productoId}")]
    public async Task<IActionResult> GetResumenProducto(int productoId)
    {
        var entradas = await _context.Movimientos
            .Where(m => m.ProductoId == productoId && m.Cantidad > 0)
            .SumAsync(m => m.Cantidad);

        var salidas = await _context.Movimientos
            .Where(m => m.ProductoId == productoId && m.Cantidad < 0)
            .SumAsync(m => m.Cantidad);

        var total = entradas + salidas;

        return Ok(new
        {
            productoId,
            entradas,
            salidas,
            total,
            mensaje = "Resumen de movimientos por producto"
        });
    }
}
