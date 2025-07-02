namespace CRM.Application.DTOs;

public class PedidoDto
{
    public int? Id { get; set; } // null ao criar
    public int ClienteId { get; set; }
    public string? Cliente { get; set; } // usado só na exibição
    public List<PedidoItemDto> Itens { get; set; } = new();
    public decimal ValorTotal => Itens?.Sum(i => i.Subtotal) ?? 0;
}
