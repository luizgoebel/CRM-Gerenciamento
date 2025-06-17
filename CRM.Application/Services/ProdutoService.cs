using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task Adicionar(ProdutoDto produtoDto)
    {
        var produto = new Produto { Nome = produtoDto.Nome, Preco = produtoDto?.Preco };
        await _produtoRepository.Adicionar(produto);
    }

    public async Task<IEnumerable<ProdutoDto>> ListarTodos()
    {
        var produtos = await _produtoRepository.ListarTodos();
        return produtos.Select(p => new ProdutoDto { Nome = p.Nome, Preco = p.Preco });
    }

    public async Task<ProdutoDto> ObterPorId(int id)
    {
        var produto = await _produtoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Produto com ID {id} não encontrado.");
        return new ProdutoDto { Nome = produto.Nome, Preco = produto.Preco };
    }

    public async Task Atualizar(int id, ProdutoDto produtoDto)
    {
        var produto = await _produtoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Produto com ID {id} não encontrado.");
        produto.Nome = produtoDto.Nome;
        produto.Preco = produtoDto.Preco;
        await _produtoRepository.Atualizar(produto);
    }

    public async Task Remover(int id)
    {
        var produto = await _produtoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Produto com ID {id} não encontrado.");
        await _produtoRepository.Remover(produto);
    }
}
