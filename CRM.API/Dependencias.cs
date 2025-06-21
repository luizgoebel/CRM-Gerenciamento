using CRM.Application.Interfaces;
using CRM.Application.Services;
using CRM.Core.Interfaces;
using CRM.Infrastructure.Repositories;

namespace CRM.API;

public static class Dependencias
{
    public static void RecuperarRepositorios(IServiceCollection services)
    {
        services.AddScoped(typeof(IClienteRepository), typeof(ClienteRepository));
        services.AddScoped(typeof(IPedidoItemRepository), typeof(PedidoItemRepository));
        services.AddScoped(typeof(IPedidoRepository), typeof(PedidoRepository));
        services.AddScoped(typeof(IProdutoRepository), typeof(ProdutoRepository));
    }
    public static void RecuperarServicos(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddTransient<IClienteService, ClienteService>();
        services.AddTransient<IPedidoItemService, PedidoItemService>();
        services.AddTransient<IPedidoService, PedidoService>();
        services.AddTransient<IProdutoService, ProdutoService>();
    }
}
