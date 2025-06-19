using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IPedidoItemRepository
{
    Task<PedidoItem?> ObterPorId(int id);
    Task<IEnumerable<PedidoItem>?> ListarPorPedido(int idPedido);
    void Adicionar(PedidoItem item);
    void Atualizar(PedidoItem item);
    void Remover(PedidoItem item);
}
