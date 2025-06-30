using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using CRM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PedidoItemRepository : IPedidoItemRepository
{
    private readonly CrmDbContext _context;

    public PedidoItemRepository(CrmDbContext context)
    {
        this._context = context;
    }

    public void Adicionar(PedidoItem item)
    {
        this._context.Set<PedidoItem>().Add(item);
        this._context.SaveChanges();
    }

    public void Atualizar(PedidoItem item)
    {
        this._context.Set<PedidoItem>().Update(item);
        this._context.SaveChanges();
    }

    public async Task<IEnumerable<PedidoItem>> ListarPorPedido(int idPedido)
    {
        return await this._context.Set<PedidoItem>()
            .Include(pi => pi.Produto)
            .Include(pi => pi.Pedido)
            .Where(pi => pi.PedidoId == idPedido)
            .ToListAsync();
    }

    public async Task<PedidoItem?> ObterPorId(int id)
    {
        return await this._context.Set<PedidoItem>().FindAsync(id);
    }

    public void Remover(PedidoItem item)
    {
        this._context.Set<PedidoItem>().Remove(item);
        this._context.SaveChanges();
    }
}
