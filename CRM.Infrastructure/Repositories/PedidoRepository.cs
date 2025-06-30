using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using CRM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly CrmDbContext _context;

    public PedidoRepository(CrmDbContext context)
    {
        this._context = context;
    }

    public void CriarPedido(Pedido pedido)
    {
        this._context.Set<Pedido>().Add(pedido);
        this._context.SaveChanges();
    }

    public async Task<Pedido?> ObterPorId(int id)
    {
        return await this._context.Set<Pedido>().FindAsync(id);
    }

    public async Task<IQueryable<Pedido>> ObterQueryPedidos()
    {
        var query = _context.Set<Pedido>()
            .Include(p => p.Cliente)
            .AsQueryable();

        return await Task.FromResult(query);
    }

    public void Remover(Pedido pedido)
    {
        this._context.Set<Pedido>().Remove(pedido);
        this._context.SaveChanges();
    }
}
