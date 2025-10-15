namespace AleStock.Api.Models;

public class Movimiento
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string Tipo { get; set; } = string.Empty; // RE-STOCK o AJUSTE
    public int Cantidad { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public Producto? Producto { get; set; }
    public Usuario? Usuario { get; set; }
}
