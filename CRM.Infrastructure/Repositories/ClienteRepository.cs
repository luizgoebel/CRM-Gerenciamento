using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    public Task<int> Adicionar(Cliente cliente)
    {
        throw new NotImplementedException();
    }

    public Task Atualizar(Cliente cliente)
    {
        throw new NotImplementedException();
    }

    public Task<Cliente?> ObterPorId(int id)
    {
        throw new NotImplementedException();
    }
}
