using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using CRM.Infrastructure.DbContext;
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
}
