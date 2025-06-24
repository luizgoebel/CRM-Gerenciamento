using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IProdutoService
{
    Task<List<ProdutoDto>> ListarTodos();
    Task<object> ObterProdutosPaginados(int page, int pageSize);
    ProdutoDto ObterPorId(int id);
    void Salvar(ProdutoDto produtoDto);
    void Remover(int id);
}
