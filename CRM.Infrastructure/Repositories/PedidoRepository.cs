using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    public Task<int> CriarPedido(Pedido pedido)
    {
        throw new NotImplementedException();
    }

    public Task<Pedido?> ObterPorId(int id)
    {
        throw new NotImplementedException();
    }
}
