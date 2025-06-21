using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using CRM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ClienteRepository : IClienteRepository
{
    private readonly CrmDbContext _context;

    public ClienteRepository(CrmDbContext context)
    {
        this._context = context;
    }

    public void Adicionar(Cliente cliente)
    {
        this._context.Set<Cliente>().Add(cliente);
        this._context.SaveChanges();
    }

    public void Atualizar(Cliente cliente)
    {
        this._context.Set<Cliente>().Update(cliente);
        this. _context.SaveChanges();
    }

    public async Task<Cliente?> ObterPorId(int id)
    {
        return await _context.Set<Cliente>().FindAsync(id);
    }

    public async Task<List<Cliente>> ObterTodosClientes()
    {
        return await _context.Set<Cliente>().ToListAsync();
    }
}