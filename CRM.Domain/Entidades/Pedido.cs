namespace CRM.Domain.Entidades;

public class Pedido : BaseModel<Pedido>
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual List<PedidoItem> Itens { get; set; } = [];

    private decimal _valorTotal;
    public decimal ValorTotal => _valorTotal;

    public void AtualizarValorTotal()
    {
        _valorTotal = Itens?.Sum(i => i.Subtotal) ?? 0m;
    }
}
