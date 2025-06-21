using CRM.Application.DTOs;
using CRM.Domain.Entidades;

namespace CRM.Application.Mappers;

public static class PedidoItemMapper
{
    public static PedidoItemDto ToDto(this PedidoItem item)
    {
        return new PedidoItemDto
        {
            Id = item.Id,
            PedidoId = item.PedidoId,
            ProdutoId = item.ProdutoId,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.Quantidade > 0 ? item.Subtotal / item.Quantidade : 0
        };
    }

    public static PedidoItem ToModel(this PedidoItemDto dto)
    {
        return new PedidoItem
        {
            Id = dto.Id,
            PedidoId = dto.PedidoId,
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade,
            Subtotal = dto.Quantidade * dto.PrecoUnitario
        };
    }
}
