namespace CRM.Application.DTOs;

public class PedidoDto
{
    public int? Id { get; set; }
    public int? ClienteId { get; set; }
    public string? Cliente { get; set; } 
    public List<PedidoItemDto>? Itens { get; set; } = new();
    public decimal? ValorTotal { get; set; } 
}
