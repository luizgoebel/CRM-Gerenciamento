using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IClienteRepository
{
    void Adicionar(Cliente cliente);
    Task<Cliente?> ObterPorId(int id);
    Task<List<Cliente>> ObterTodosClientes();
    void Atualizar(Cliente cliente);
}
