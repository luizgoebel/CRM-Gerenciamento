using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IPedidoRepository
{
    Task<int> CriarPedido(Pedido pedido);
    Task<Pedido?> ObterPorId(int id);
}
