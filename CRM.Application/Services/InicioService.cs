using CRM.Application.DTOs;
using CRM.Application.Interfaces;

namespace CRM.Application.Services;

public class InicioService : IInicioService
{
    private readonly IClienteService _clienteService;
    private readonly IProdutoService _produtoService;
    private readonly IPedidoService _pedidoService;

    public InicioService(
        IClienteService clienteService,
        IProdutoService produtoService,
        IPedidoService pedidoService)
    {
        this._clienteService = clienteService;
        this._produtoService = produtoService;
        this._pedidoService = pedidoService;
    }

    public async Task<DashboardDto> ObterDadosDashboard()
    {
        DateOnly dataHoje = DateOnly.FromDateTime(DateTime.Now);
       
        int totalClientes = await _clienteService.ObterTotalClientes();
        int totalProdutos = await _produtoService.ObterTotalProdutos();
        int totalPedidosHoje = await _pedidoService.ObterTotalPedidosNaData(dataHoje);

        return new DashboardDto(totalClientes, totalProdutos, totalPedidosHoje);
    }

}