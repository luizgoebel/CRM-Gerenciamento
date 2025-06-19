using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IPedidoRepository
{
    void CriarPedido(Pedido pedido);
    Task<Pedido?> ObterPorId(int id);
}
