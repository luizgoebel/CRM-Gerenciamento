namespace CRM.Application.DTOs;

public class PedidoDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string? Cliente { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public List<PedidoItemDto> Itens { get; set; } = new();
}
