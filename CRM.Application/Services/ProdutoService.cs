﻿using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
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
        Produto produto = new() { Nome = produtoDto.Nome, Preco = produtoDto.Preco };
        Validar(produto);
        this._produtoRepository.Adicionar(produto);
    }

    public async Task<IEnumerable<ProdutoDto>> ListarTodos()
    {
        IEnumerable<Produto> produtos = [];
        produtos = await this._produtoRepository.ListarTodos();

        if (produtos == null || !produtos.Any())
            throw new ServiceException("Nenhum produto encontrado.");

        return produtos.Select(p => p.ToDto());
    }

    public ProdutoDto ObterPorId(int id)
    {
        Produto produto = RecuperarProduto(id);
        ProdutoDto produtoDto = produto.ToDto();
        return produtoDto;
    }

    public void Atualizar(ProdutoDto produtoDto)
    {
        Produto produto = RecuperarProduto(produtoDto.Id);
        produto.Alterar(produtoDto.Nome, produtoDto.Preco);
        Validar(produto);
        this._produtoRepository.Atualizar(produto);
    }

    public void Remover(int id)
    {
        Produto produto = RecuperarProduto(id);
        this._produtoRepository.Remover(produto);
    }

    private void Validar(Produto produto)
    {
        ValidationResult resultado = produto.Validar();

        if (!resultado.IsValid)
            throw new DomainException(resultado.Erros.First());
    }

    private Produto RecuperarProduto(int id)
    {
        return this._produtoRepository.ObterPorId(id).GetAwaiter().GetResult()
            ?? throw new ServiceException($"Produto não encontrado.");
    }
}
