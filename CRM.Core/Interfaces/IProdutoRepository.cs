using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IProdutoRepository
{
    void Adicionar(Produto produto);
    List<Produto> ListarTodos();
    Task<IQueryable<Produto>> ObterQueryProdutos();
    Task<Produto?> ObterPorId(int id);
    void Atualizar(Produto produto);
    void Remover(Produto produto);
    Task<int> ObterTotalProdutos();
}
