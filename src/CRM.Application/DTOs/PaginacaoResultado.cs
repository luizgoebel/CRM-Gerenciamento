namespace CRM.Application.DTOs;

public class PaginacaoResultado<T>
{
    public List<T> Itens { get; set; } = new();
    public int Total { get; set; }
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
}
