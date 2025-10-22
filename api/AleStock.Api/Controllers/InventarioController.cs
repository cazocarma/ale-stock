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

    // 1️⃣ Listar todo el inventario
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inventario = await _context.Inventarios
            .Include(i => i.Producto)
            .OrderBy(i => i.Producto!.Sku)
            .ToListAsync();

        return Ok(inventario);
    }

    // 2️⃣ Obtener un producto específico
    [HttpGet("{productoId}")]
    public async Task<IActionResult> GetByProducto(int productoId)
    {
        var inventario = await _context.Inventarios
            .Include(i => i.Producto)
            .FirstOrDefaultAsync(i => i.ProductoId == productoId);

        if (inventario == null)
            return NotFound($"No existe inventario para el producto {productoId}");

        return Ok(inventario);
    }

    // 3️⃣ Ajustar stock manual (entrada/salida/ajuste)
    [HttpPost("ajustar")]
    public async Task<IActionResult> AjustarStock([FromBody] Movimiento movimiento)
    {
        var inventario = await _context.Inventarios
            .FirstOrDefaultAsync(i => i.ProductoId == movimiento.ProductoId);

        if (inventario == null)
            return NotFound($"No existe inventario para el producto {movimiento.ProductoId}");

        var nuevoStock = inventario.Cantidad + movimiento.Cantidad;

        // Evitar stock negativo
        if (nuevoStock < 0)
            return BadRequest($"El ajuste dejaría el stock negativo para el producto {movimiento.ProductoId}");

        inventario.Cantidad = nuevoStock;

        // Determinar tipo de movimiento
        var tipoMovimiento = movimiento.Cantidad > 0 ? "RE-STOCK" : "AJUSTE";

        var nuevoMovimiento = new Movimiento
        {
            ProductoId = movimiento.ProductoId,
            Tipo = tipoMovimiento,
            Cantidad = movimiento.Cantidad,
            Comentario = movimiento.Comentario,
            UsuarioId = movimiento.UsuarioId,
            Fecha = DateTime.UtcNow
        };

        _context.Movimientos.Add(nuevoMovimiento);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Inventario actualizado correctamente",
            inventario,
            movimiento = nuevoMovimiento
        });
    }

    // 4️⃣ Listar productos con stock bajo el mínimo
    [HttpGet("bajo-minimo/{minimo}")]
    [Authorize(Roles = "Bodega,Coordinador")]
    public async Task<IActionResult> GetStockBajoMinimo(int minimo)
    {
        var bajos = await _context.Inventarios
            .Include(i => i.Producto)
            .Where(i => i.Cantidad < minimo)
            .OrderBy(i => i.Cantidad)
            .ToListAsync();

        return Ok(bajos);
    }
}
