namespace CRM.Domain.Entidades;

public class Pedido : BaseModel<Pedido>
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual List<PedidoItem> Itens { get; set; } = [];
    public decimal ValorTotal { get; private set; }

    public void AtualizarValorTotal()
    {
        ValorTotal = Itens?.Sum(i => i.Subtotal) ?? 0m;
    }
}
