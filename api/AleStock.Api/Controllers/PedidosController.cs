using AleStock.Api.Data;
using AleStock.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AleStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AleStockDbContext _context;

    public PedidosController(AleStockDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Crear pedido (solo Coordinador)
    [HttpPost]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> CrearPedido([FromBody] Pedido pedido)
    {
        if (pedido.Detalles == null || !pedido.Detalles.Any())
            return BadRequest("El pedido debe incluir al menos un detalle.");

        pedido.Fecha = DateTime.UtcNow;
        pedido.Estado = "Pendiente";

        // Guarda el pedido
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        // Crea movimientos automáticos para los productos del pedido
        foreach (var detalle in pedido.Detalles)
        {
            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.ProductoId == detalle.ProductoId);

            if (inventario == null)
                continue;

            inventario.Cantidad -= detalle.Cantidad; // descuenta stock

            var movimiento = new Movimiento
            {
                ProductoId = detalle.ProductoId,
                Tipo = "SALIDA",
                Cantidad = -detalle.Cantidad,
                Comentario = $"Salida por pedido #{pedido.Id}",
                UsuarioId = pedido.CreadoPorId,
                Fecha = DateTime.UtcNow
            };

            _context.Movimientos.Add(movimiento);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Pedido creado correctamente.",
            pedido
        });
    }

    // 2️⃣ Listar pedidos (solo Bodega)
    [HttpGet]
    [Authorize(Roles = "Bodega")]
    public async Task<IActionResult> ObtenerPedidos()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
            .Include(p => p.CreadoPor)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();

        return Ok(pedidos);
    }

    // 3️⃣ Actualizar estado (solo Bodega)
    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "Bodega")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] string nuevoEstado)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null)
            return NotFound();

        pedido.Estado = nuevoEstado;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = $"Pedido #{id} actualizado a estado {nuevoEstado}.",
            pedido
        });
    }

    // 4️⃣ Aprobación de pedido (rol Coordinador)
    [HttpPatch("{id}/aprobar")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> AprobarPedido(int id, [FromBody] string comentario)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Estado = "Aprobado";
        pedido.Comentario = comentario;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Pedido aprobado", pedido });
    }

}
