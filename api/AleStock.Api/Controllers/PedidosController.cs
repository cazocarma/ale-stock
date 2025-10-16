using AleStock.Api.Data;
using AleStock.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AleStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly AleStockDbContext _context;

    public PedidosController(AleStockDbContext context)
    {
        _context = context;
    }

    // GET: api/pedidos
    [HttpGet]
    public async Task<IActionResult> GetPedidos()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .ToListAsync();

        return Ok(pedidos);
    }

    // POST: api/pedidos
    [Authorize(Roles = "Coordinador")]
    [HttpPost]
    public async Task<IActionResult> CreatePedido([FromBody] Pedido pedido)
    {
        pedido.Fecha = DateTime.UtcNow;
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return Ok(pedido);
    }

    // PUT: api/pedidos/{id}/estado
    [Authorize(Roles = "Bodega,Coordinador")]
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] string estado)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null) return NotFound();

        pedido.Estado = estado;
        await _context.SaveChangesAsync();

        return Ok(pedido);
    }
}
