namespace CRM.Domain.Entidades;

public class PedidoItem : BaseModel<PedidoItem>
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal { get; private set; }

    public virtual Produto? Produto { get; set; }
    public virtual Pedido? Pedido { get; set; }

    public void AtualizarValores(Produto produto)
    {
        if (produto == null) throw new ArgumentNullException(nameof(produto));

        PrecoUnitario = produto.Preco;
        Subtotal = Quantidade * PrecoUnitario;
    }

    public ValidationResult Validar()
    {
        ValidationResult result = new();

        if (ProdutoId <= 0 || Quantidade <= 0 || Subtotal <= 0)
            result.AddError("Pedido inválido.");

        return result;
    }
}
