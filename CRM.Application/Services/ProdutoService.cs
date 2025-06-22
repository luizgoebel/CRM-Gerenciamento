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

    public async Task<List<ProdutoDto>> ListarTodos()
    {
        IEnumerable<Produto> produtos = [];
        produtos = await this._produtoRepository.ListarTodos();

        return produtos.Select(p => p.ToDto()).ToList();
    }

    public ProdutoDto ObterPorId(int id)
    {
        Produto produto = RecuperarProduto(id);
        ProdutoDto produtoDto = produto.ToDto();
        return produtoDto;
    }

    public void Salvar(ProdutoDto produtoDto)
    {
        if (produtoDto == null)
            throw new NullReferenceException("O DTO do produto não pode ser nulo.");

        Validar(produtoDto.ToModel());

        if (produtoDto.Id > 0)
        {
            Produto produto = RecuperarProduto(produtoDto.Id ?? 0);
            produto.Alterar(produtoDto.Nome, produtoDto.Preco);
            this._produtoRepository.Atualizar(produto);
            return;
        }

        this._produtoRepository.Adicionar(produtoDto.ToModel());
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
