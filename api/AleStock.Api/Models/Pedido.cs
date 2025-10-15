namespace AleStock.Api.Models;

public class Pedido
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "Pendiente";
    public string Comentario { get; set; } = string.Empty;
    public int CreadoPorId { get; set; }

    public Usuario? CreadoPor { get; set; }
    public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
}
