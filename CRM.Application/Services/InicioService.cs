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
        int totalClientes = await _clienteService.ObterTotalClientes ?? 0;
        int totalProdutos = await _produtoService.ObterTotalProdutos() ?? 0;
        int totalPedidosHoje = await _pedidoService.ObterTotalPedidosHoje() ?? 0;

        DashboardDto dashboard = new(totalClientes, totalProdutos, totalPedidosHoje);
        return dashboard;
    }
}