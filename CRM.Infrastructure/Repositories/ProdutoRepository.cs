using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    public Task Adicionar(Produto produto)
    {
        throw new NotImplementedException();
    }

    public Task Atualizar(Produto produto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Produto>> ListarTodos()
    {
        throw new NotImplementedException();
    }

    public Task<Produto?> ObterPorId(int id)
    {
        throw new NotImplementedException();
    }

    public Task Remover(Produto produto)
    {
        throw new NotImplementedException();
    }
}
