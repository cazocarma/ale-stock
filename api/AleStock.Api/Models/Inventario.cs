namespace AleStock.Api.Models;

public class Inventario
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public string Ubicacion { get; set; } = "BODEGA";

    public Producto? Producto { get; set; }
}
