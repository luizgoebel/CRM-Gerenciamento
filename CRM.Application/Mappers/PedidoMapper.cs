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
            Itens = pedido.Itens.Select(i => i.ToDto()).ToList()
        };
    }

    public static Pedido ToModel(this PedidoDto dto)
    {
        return new Pedido
        {
            Id = dto.Id,
            ClienteId = dto.ClienteId,
            Itens = dto.Itens.Select(i => i.ToModel()).ToList()
        };
    }
}
