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
            PrecoUnitario = item.PrecoUnitario
        };
    }

    public static PedidoItem ToModel(this PedidoItemDto dto)
    {
        return new PedidoItem(
            dto.PedidoId,
            dto.ProdutoId,
            dto.Quantidade,
            dto.PrecoUnitario
        );
    }
}
