using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IPedidoService
{
    void CriarPedido(PedidoDto pedidoDto);
    Task<PedidoDto> ObterPorId(int id);
    Task<PaginacaoResultado<PedidoDto>> ObterPedidosPaginados(string filtro, int page, int pageSize);
}
