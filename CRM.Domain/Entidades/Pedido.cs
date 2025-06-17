namespace CRM.Domain.Entidades;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public List<PedidoItem> Itens { get; set; } = new();
    public decimal ValorTotal => Itens.Sum(i => i.Subtotal);
}
