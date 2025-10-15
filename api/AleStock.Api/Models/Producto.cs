namespace AleStock.Api.Models;

public class Producto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Estado { get; set; } = "activo"; // activo / descontinuado
}
