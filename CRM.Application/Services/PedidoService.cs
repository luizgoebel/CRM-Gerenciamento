using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;

    public PedidoService(IPedidoRepository pedidoRepository, IClienteRepository clienteRepository)
    {
        this._pedidoRepository = pedidoRepository;
        this._clienteRepository = clienteRepository;
    }

    public async void CriarPedido(PedidoDto pedidoDto)
    {
        Cliente cliente = await this._clienteRepository.ObterPorId(pedidoDto.ClienteId) ??
            throw new ServiceException($"Cliente não encontrado.");

        Pedido pedido = pedidoDto.ToModel();
        pedido.AssociarCliente(cliente);

        ValidarPedido(pedido);
        this._pedidoRepository.CriarPedido(pedido);
    }

    public async Task<PaginacaoResultado<PedidoDto>> ObterPedidosPaginados(string filtro, int page, int pageSize)
    {
        IQueryable<Pedido> query = await this._pedidoRepository.ObterQueryPedidos();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();
            query = query.Where(c => c.Cliente!.Nome.ToLower().Contains(filtro));
        }

        // Ordena pela DataCriacao do mais recente para o mais antigo, depois pelo Nome
        query = query
            .OrderByDescending(c => c.DataCriacao)
            .ThenBy(c => c.Cliente!.Nome);

        if (!query.Any())
            return new PaginacaoResultado<PedidoDto>();

        int total = query.Count();
        List<Pedido> pedidos = [.. query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)];

        List<PedidoDto> pedidosDto = [.. pedidos.Select(c => c.ToDto())];

        return new PaginacaoResultado<PedidoDto>
        {
            Itens = pedidosDto,
            Total = total,
            PaginaAtual = page,
            TotalPaginas = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    public async Task<PedidoDto> ObterPorId(int id)
    {
        Pedido pedido = await this._pedidoRepository.ObterPorId(id)
        ?? throw new ServiceException($"Pedido não encontrado.");

        PedidoDto pedidoDto = pedido.ToDto();

        return pedidoDto;
    }

    private void ValidarPedido(Pedido pedido)
    {
        if (pedido.Itens == null || pedido.Itens.Count < 1)
            throw new ServiceException("Pedido deve conter pelo menos um item.");

        var produtosDuplicados = pedido.Itens
            .GroupBy(i => i.ProdutoId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (produtosDuplicados.Any())
            throw new ServiceException("Não é permitido adicionar o mesmo produto mais de uma vez no pedido.");

        if(pedido.Cliente == null)
            throw new ServiceException("Não há um cliente associado ao pedido.");

        if (!pedido.Cliente.CadastroCompleto())
            throw new ServiceException("Cadastro incompleto. Atualize o cadastro do cliente para prosseguir com o pedido.");

        decimal somaItens = pedido.Itens.Sum(i => i.Subtotal);
        if (pedido.ValorTotal != somaItens)
            throw new ServiceException("O valor total do pedido está inconsistente com a soma dos subtotais dos itens.");
    }
}
