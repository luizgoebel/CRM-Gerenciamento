namespace CRM.Domain.Entidades;

public class PedidoItem : BaseModel<PedidoItem>
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal Subtotal { get; set; }
    public Produto? Produto { get; set; }

    public ValidationResult Validar()
    {
        ValidationResult result = new();

        if (this.PedidoId <= 0 || this.ProdutoId <= 0 || this.Quantidade <= 0 || this.Subtotal <= 0)
            result.AddError("Pedido inválido.");

        return result;
    }

    public void Alterar(int pedidoId, int produtoId, int quantidade, decimal subtotal)
    {
        this.PedidoId = pedidoId;
        this.ProdutoId = produtoId;
        this.Quantidade = quantidade;
        this.Subtotal = subtotal;
    }
}
