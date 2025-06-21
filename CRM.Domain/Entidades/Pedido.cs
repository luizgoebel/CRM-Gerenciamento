namespace CRM.Domain.Entidades;

public class Pedido : BaseModel<Pedido>
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual List<PedidoItem> Itens { get; set; } = new();

    public decimal ValorTotal => Itens?.Sum(i => i.Subtotal) ?? 0m;

    public void AssociarCliente(Cliente cliente)
    {
        Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
        ClienteId = cliente.Id;
    }
}
