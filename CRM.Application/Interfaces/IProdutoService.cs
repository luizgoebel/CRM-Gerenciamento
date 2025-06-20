using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IProdutoService
{
    void Adicionar(ProdutoDto produtoDto);
    Task<IEnumerable<ProdutoDto>> ListarTodos();
    ProdutoDto ObterPorId(int id);
    void Atualizar(ProdutoDto produtoDto);
    void Remover(int id);
}
