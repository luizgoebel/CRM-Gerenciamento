namespace CRM.Application.DTOs;

public class DashboardDto
{
    public DashboardDto(int totalClientesAtivos, int totalProdutosEstoque, int totalPedidosHoje)
    {
        TotalClientesAtivos = totalClientesAtivos;
        TotalProdutosEstoque = totalProdutosEstoque;
        TotalPedidosHoje = totalPedidosHoje;
    }
    public DashboardDto() { }

    public int TotalClientesAtivos { get; set; }
    public int TotalProdutosEstoque { get; set; }
    public int TotalPedidosHoje { get; set; }

}