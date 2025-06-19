using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IPedidoItemService
{
    void Adicionar(PedidoItemDto dto);
    void Atualizar(PedidoItemDto dto);
    void Remover(int id);
    Task<PedidoItemDto> ObterPorId(int id);
    Task<IEnumerable<PedidoItemDto>> ListarPorPedido(int pedidoId);
}
