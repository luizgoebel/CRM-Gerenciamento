using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    public Task Adicionar(Cliente cliente)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExisteTelefone(string telefone)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Cliente>> ListarTodos()
    {
        throw new NotImplementedException();
    }

    public Task<Cliente?> ObterPorId(int id)
    {
        throw new NotImplementedException();
    }
}
