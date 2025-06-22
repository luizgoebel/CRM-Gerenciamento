using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IClienteService
{
    Task<ClienteDto> ObterPorId(int id);
    Task<List<ClienteDto>> ObterTodosClientes();
    void Salvar(ClienteDto clienteDto);
}
