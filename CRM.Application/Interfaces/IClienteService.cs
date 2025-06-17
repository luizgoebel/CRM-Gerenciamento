using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IClienteService
{
    Task<int> Adicionar(ClienteDto clienteDto);
    Task<ClienteDto> ObterPorId(int id);
    Task Atualizar(int id, ClienteDto clienteDto);
}
