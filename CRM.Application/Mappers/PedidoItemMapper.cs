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
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario
        };
    }

    public static PedidoItem ToModel(this PedidoItemDto dto)
    {
        return new PedidoItem
        {
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade,
            PrecoUnitario = dto.PrecoUnitario
        };
    }
}


