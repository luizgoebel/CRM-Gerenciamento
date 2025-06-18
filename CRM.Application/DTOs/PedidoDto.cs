namespace CRM.Application.DTOs;

public class PedidoDto
{
    public int? Id { get; set; }
    public int ClienteId { get; set; }
    public List<PedidoItemDto> Itens { get; set; } = new();
}
