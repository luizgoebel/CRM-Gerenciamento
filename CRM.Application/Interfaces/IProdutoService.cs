using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IProdutoService
{
    Task<IEnumerable<ProdutoDto>> ListarTodos();
    ProdutoDto ObterPorId(int id);
    void Salvar(ProdutoDto produtoDto);
    void Remover(int id);
}
