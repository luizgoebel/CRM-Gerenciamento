using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IPedidoService
{
    Task<int> CriarPedido(PedidoDto pedidoDto);
    Task<PedidoDto> ObterPorId(int id);
}
