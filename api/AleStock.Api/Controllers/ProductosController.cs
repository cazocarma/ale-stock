using AleStock.Api.Data;
using AleStock.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AleStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly AleStockDbContext _context;

    public ProductosController(AleStockDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Listar todos los productos
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productos = await _context.Productos
            .OrderBy(p => p.Sku)
            .ToListAsync();

        return Ok(productos);
    }

    // 2️⃣ Obtener un producto por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
            return NotFound(new {
                mensaje = $"No se encontró el producto con ID {id}"
            });

        return Ok(producto);
    }

    // 3️⃣ Crear producto (solo Coordinador)
    [HttpPost]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Crear([FromBody] Producto producto)
    {
        if (await _context.Productos.AnyAsync(p => p.Sku == producto.Sku))
            return BadRequest(new {
                mensaje = $"Ya existe un producto con SKU {producto.Sku}"
            });

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        // Vincular automáticamente al inventario
        var inventario = new Inventario
        {
            ProductoId = producto.Id,
            Cantidad = 0,
            Ubicacion = "BODEGA"
        };

        _context.Inventarios.Add(inventario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, new
        {
            producto,
            inventario
        });
    }

    // 4️⃣ Actualizar producto (solo Coordinador)
    [HttpPut("{id}")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] Producto producto)
    {
        var existente = await _context.Productos.FindAsync(id);
        if (existente == null)
            return NotFound(new {
                mensaje = $"No se encontró el producto con ID {id}"
            });

        existente.Sku = producto.Sku;
        existente.Marca = producto.Marca;
        existente.Modelo = producto.Modelo;
        existente.Estado = producto.Estado;

        await _context.SaveChangesAsync();
        return Ok(existente);
    }

    // 5️⃣ Eliminar producto (solo Coordinador)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
            return NotFound(new {
                mensaje = $"No se encontró el producto con ID {id}"
            });

        // Eliminar también del inventario
        var inventario = await _context.Inventarios
            .FirstOrDefaultAsync(i => i.ProductoId == producto.Id);

        if (inventario != null)
            _context.Inventarios.Remove(inventario);

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return Ok(new {
            mensaje = $"Producto {id} eliminado correctamente junto a su inventario asociado",
            eliminado = true
        });
    }

    // 6️⃣ Buscar por texto (SKU, Marca o Modelo)
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new {
                mensaje = "Debe ingresar un texto de búsqueda"
            });

        var productos = await _context.Productos
            .Where(p =>
                p.Sku.ToLower().Contains(q.ToLower()) ||
                p.Marca.ToLower().Contains(q.ToLower()) ||
                p.Modelo.ToLower().Contains(q.ToLower()))
            .OrderBy(p => p.Sku)
            .ToListAsync();

        return Ok(productos);
    }
}
