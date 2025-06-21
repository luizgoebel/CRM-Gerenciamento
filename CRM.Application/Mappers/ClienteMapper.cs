using CRM.Application.DTOs;
using CRM.Domain.Entidades;

namespace CRM.Application.Mappers;

public static class ClienteMapper
{
    public static ClienteDto ToDto(this Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nome = cliente.Nome,
        Telefone = cliente.Telefone,
        Email = cliente.Email,
        Endereco = cliente.Endereco,
    };

    public static Cliente ToModel(this ClienteDto dto) => new()
    {
        Id = dto.Id,
        Nome = dto.Nome,
        Telefone = dto.Telefone,
        Email = dto.Email,
        Endereco = dto.Endereco,
    };
}

