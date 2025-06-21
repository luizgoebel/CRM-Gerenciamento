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

    public static Produto ToModel(this ProdutoDto dto)
    {
        return new Produto
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Preco = dto.Preco
        };
    }
}
