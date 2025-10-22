namespace AleStock.Api.Models;

public class Movimiento
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public int UsuarioId { get; set; }

    private DateTime _fecha = DateTime.UtcNow;
    public DateTime Fecha
    {
        get => _fecha;
        set => _fecha = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public Producto? Producto { get; set; }
    public Usuario? Usuario { get; set; }
}
