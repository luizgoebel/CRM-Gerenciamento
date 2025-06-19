using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IProdutoRepository
{
    void Adicionar(Produto produto);
    Task<IEnumerable<Produto>> ListarTodos();
    Task<Produto?> ObterPorId(int id);
    void Atualizar(Produto produto);
    void Remover(Produto produto);
}
