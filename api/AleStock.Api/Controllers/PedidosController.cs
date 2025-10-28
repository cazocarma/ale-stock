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

    // 1️⃣ Crear pedido (rol Coordinador)
    [HttpPost]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> CrearPedido([FromBody] Pedido pedido)
    {
        if (pedido.CreadoPorId <= 0)
            return BadRequest(new { mensaje = "El pedido debe tener un usuario creador válido." });

        pedido.Fecha = DateTime.UtcNow;
        pedido.Estado = "Pendiente";

        // 🔹 Asociar el pedido a cada detalle
        if (pedido.Detalles != null)
        {
            foreach (var d in pedido.Detalles)
            {
                d.Pedido = pedido;
            }
        }

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return Ok(pedido);
    }


    // 2️⃣ Listar pedidos
    [HttpGet]
    [Authorize(Roles = "Bodega,Coordinador")]
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

    // 3️⃣ Aprobar pedido (rol Coordinador)
    [HttpPatch("{id}/aprobar")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> AprobarPedido(int id, [FromBody] string comentario)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null) return NotFound();

        pedido.Estado = "Aprobado";
        pedido.Comentario = comentario;

        foreach (var detalle in pedido.Detalles)
        {
            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.ProductoId == detalle.ProductoId);

            if (inventario != null)
            {
                inventario.Cantidad -= detalle.Cantidad;

                _context.Movimientos.Add(new Movimiento
                {
                    ProductoId = detalle.ProductoId,
                    Tipo = "SALIDA",
                    Cantidad = detalle.Cantidad,
                    Comentario = $"Pedido #{pedido.Id} aprobado - {comentario}",
                    UsuarioId = pedido.CreadoPorId,
                    Fecha = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Pedido aprobado y stock actualizado", pedido });
    }

    // 4️⃣ Rechazar pedido (rol Coordinador)
    [HttpPatch("{id}/rechazar")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> RechazarPedido(int id, [FromBody] string motivo)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Estado = "Rechazado";
        pedido.Comentario = motivo;

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Pedido rechazado", pedido });
    }

   // 5️⃣ Actualizar estado y comentario (rol Bodega)
    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "Bodega")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] PedidoEstadoUpdateRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Estado))
            return BadRequest(new { mensaje = "Debe indicar el estado." });

        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null)
            return NotFound(new { mensaje = $"No se encontró el pedido con ID {id}" });

        pedido.Estado = request.Estado;
        pedido.Comentario = request.Comentario;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Estado y comentario actualizados correctamente",
            pedido
        });
    }

    // DTO para el cuerpo del request
    public class PedidoEstadoUpdateRequest
    {
        public string Estado { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
    }

}
