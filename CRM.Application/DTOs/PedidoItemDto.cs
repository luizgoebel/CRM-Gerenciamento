namespace CRM.Application.DTOs;

public class PedidoItemDto
{
    public int? Id { get; set; } // pode ser nulo em novos pedidos
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
