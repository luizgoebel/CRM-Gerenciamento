namespace CRM.Domain.Entidades;

public class PedidoItem : BaseModel<PedidoItem>
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }

    private decimal _precoUnitario;
    private decimal _subtotal;

    public decimal PrecoUnitario => _precoUnitario;
    public decimal Subtotal => _subtotal;

    public virtual Produto? Produto { get; set; }
    public virtual Pedido? Pedido { get; set; }

    public void AtualizarValores(Produto produto)
    {
        Validar();
        _precoUnitario = produto?.Preco ?? 0m;
        _subtotal = Quantidade * _precoUnitario;
    }

    public ValidationResult Validar()
    {
        ValidationResult result = new();

        if (this.ProdutoId <= 0 || this.Quantidade <= 0 || this.Subtotal <= 0)
            result.AddError("Pedido inválido.");

        return result;
    }
}
