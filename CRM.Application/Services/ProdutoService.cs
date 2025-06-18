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
        try
        {
            var produto = new Produto { Nome = produtoDto.Nome!, Preco = produtoDto?.Preco };
            await _produtoRepository.Adicionar(produto);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao adicionar produto.", ex);
        }

    }

    public async Task<IEnumerable<ProdutoDto>> ListarTodos()
    {
        try
        {
            var produtos = await _produtoRepository.ListarTodos();
            return produtos.Select(p => new ProdutoDto { Nome = p.Nome, Preco = p.Preco });
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao listar produtos.", ex);
        }
    }

    public async Task<ProdutoDto> ObterPorId(int id)
    {
        try
        {
            var produto = await _produtoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Produto com ID {id} não encontrado.");
            return new ProdutoDto { Id = produto.Id, Nome = produto.Nome, Preco = produto.Preco };
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao obter produto por ID.", ex);
        }
    }

    public async Task Atualizar(int id, ProdutoDto produtoDto)
    {        
        try
        {
            var produto = await _produtoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Produto com ID {id} não encontrado.");
            produto.Nome = produtoDto.Nome!;
            produto.Preco = produtoDto.Preco;
            await _produtoRepository.Atualizar(produto);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao atualizar produto.", ex);
        }
    }

    public async Task Remover(int id)
    {
        try
        {
            var produto = await _produtoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Produto com ID {id} não encontrado.");
            await _produtoRepository.Remover(produto);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao remover produto.", ex);
        }
    }
}
