using CRM.Application.DTOs;
using CRM.Domain.Entidades;

namespace CRM.Application.Mappers;

public static class PedidoMapper
{
    public static PedidoDto ToDto(this Pedido pedido)
    {
        return new PedidoDto
        {
            Id = pedido.Id,
            ClienteId = pedido.ClienteId,
            Cliente = pedido.Cliente?.Nome,
            Itens = pedido.Itens?.Select(i => i.ToDto()).ToList(),
            ValorTotal = pedido.ValorTotal
        };
    }

    public static Pedido ToModel(this PedidoDto dto)
    {
        var pedido = new Pedido
        {
            Id = dto.Id ?? 0,
            ClienteId = dto.ClienteId ?? 0,
            Itens = dto.Itens?.Select(i => i.ToModel()).ToList() ?? new()
        };
        pedido.AtualizarValorTotal();
        return pedido;
    }
}

