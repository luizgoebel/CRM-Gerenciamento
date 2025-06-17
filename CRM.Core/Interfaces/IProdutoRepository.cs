using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IProdutoRepository
{
    Task Adicionar(Produto produto);
    Task<IEnumerable<Produto>> ListarTodos();
    Task<Produto?> ObterPorId(int id);
    Task Atualizar(Produto produto);
    Task Remover(Produto produto);
}
