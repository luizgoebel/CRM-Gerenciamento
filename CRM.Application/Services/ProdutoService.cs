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
        this._produtoRepository = produtoRepository;
    }

    public void Adicionar(ProdutoDto produtoDto)
    {
        Produto produto = new() { Nome = produtoDto.Nome, Preco = produtoDto?.Preco };
        this._produtoRepository.Adicionar(produto);
    }

    public async Task<IEnumerable<ProdutoDto>> ListarTodos()
    {
        IEnumerable<Produto> produtos = [];
        produtos = await this._produtoRepository.ListarTodos();
        return produtos.Select(p => new ProdutoDto { Nome = p.Nome, Preco = p.Preco });
    }

    public async Task<ProdutoDto> ObterPorId(int id)
    {
            Produto produto = await this._produtoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Produto não encontrado.");
            return new ProdutoDto { Id = produto.Id, Nome = produto.Nome, Preco = produto.Preco };
    }

    public void Atualizar(ProdutoDto produtoDto)
    {
            Produto produto = this._produtoRepository.ObterPorId(produtoDto.Id).GetAwaiter().GetResult()
            ?? throw new ServiceException($"Produto não encontrado.");
            produto.Nome = produtoDto.Nome!;
            produto.Preco = produtoDto.Preco;
            this._produtoRepository.Atualizar(produto);
    }

    public void Remover(int id)
    {
            Produto produto = this._produtoRepository.ObterPorId(id).GetAwaiter().GetResult()
            ?? throw new ServiceException($"Produto não encontrado.");
            this._produtoRepository.Remover(produto);
    }
}
