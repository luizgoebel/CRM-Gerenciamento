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
            ProdutoId = item.ProdutoId,
            PedidoId = item.PedidoId,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario,
            Subtotal = item.Subtotal
        };
    }

    public static PedidoItem ToModel(this PedidoItemDto dto)
    {
        return new PedidoItem
        {
            Id = dto.Id ?? 0,
            PedidoId = dto.PedidoId ?? 0,
            ProdutoId = dto.ProdutoId ?? 0,
            Quantidade = dto.Quantidade ?? 0
        };
    }
}


