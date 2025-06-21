namespace CRM.Domain.Entidades;

public class PedidoItem : BaseModel<PedidoItem>
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; private set; }

    // Subtotal is now calculated from Quantidade and PrecoUnitario
    public decimal Subtotal => Quantidade * PrecoUnitario;

    public virtual Produto? Produto { get; set; }
    public virtual Pedido? Pedido { get; set; }

    // Add a constructor or method to set PrecoUnitario
    public PedidoItem() { }

    public PedidoItem(int pedidoId, int produtoId, int quantidade, decimal precoUnitario)
    {
        PedidoId = pedidoId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public ValidationResult Validar()
    {
        ValidationResult result = new();

        if (this.PedidoId <= 0 || this.ProdutoId <= 0 || this.Quantidade <= 0 || this.Subtotal <= 0)
            result.AddError("Pedido inválido.");

        return result;
    }

    public void Alterar(int pedidoId, int produtoId, int quantidade, decimal precoUnitario)
    {
        PedidoId = pedidoId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }
}
