using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IClienteService
{
    void Adicionar(ClienteDto clienteDto);
    Task<ClienteDto> ObterPorId(int id);
    void Atualizar(ClienteDto clienteDto);
}
