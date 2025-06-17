using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IProdutoService
{
    Task Adicionar(ProdutoDto produtoDto);
    Task<IEnumerable<ProdutoDto>> ListarTodos();
    Task<ProdutoDto> ObterPorId(int id);
    Task Atualizar(int id, ProdutoDto produtoDto);
    Task Remover(int id);
}
