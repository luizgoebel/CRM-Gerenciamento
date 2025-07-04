﻿using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using CRM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly CrmDbContext _context;

    public ProdutoRepository(CrmDbContext context)
    {
        this._context = context;
    }

    public void Adicionar(Produto produto)
    {
        this._context.Set<Produto>().Add(produto);
        this._context.SaveChanges();
    }

    public void Atualizar(Produto produto)
    {
        this._context.Set<Produto>().Update(produto);
        this._context.SaveChanges();
    }

    public void Remover(Produto produto)
    {
        this._context.Set<Produto>().Remove(produto);
        this._context.SaveChanges();
    }

    public List<Produto> ListarTodos()
    {
        return this._context.Set<Produto>().ToList();
    }

    public async Task<Produto?> ObterPorId(int id)
    {
        return await this._context.Set<Produto>().FindAsync(id);
    }

    public async Task<IQueryable<Produto>> ObterQueryProdutos()
    {
        return await Task.FromResult(_context.Set<Produto>().OrderBy(c => c.Nome).AsQueryable());
    }

    public async Task<int> ObterTotalProdutos()
    {
        return await _context.Set<Produto>().CountAsync();
    }
}
