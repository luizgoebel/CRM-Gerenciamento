using CRM.Application.DTOs;
using CRM.Domain.Entidades;

namespace CRM.Application.Mappers;

public static class ProdutoMapper
{
    public static ProdutoDto ToDto(this Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco
        };
    }

    public static Produto ToModel(this ProdutoDto dto) => new()
    {
        Id = dto.Id ?? 0,
        Nome = dto.Nome?? string.Empty,
        Preco = dto.Preco ?? 0
    };
}
