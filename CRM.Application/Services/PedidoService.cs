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

    public void CriarPedido(PedidoDto pedidoDto)
    {
        if (pedidoDto == null) throw new ArgumentNullException(nameof(pedidoDto));

        // Buscar cliente
        var cliente = _clienteRepository.ObterPorId(pedidoDto.ClienteId).GetAwaiter().GetResult()
            ?? throw new ServiceException("Cliente não encontrado.");

        // Mapear para entidade
        var pedido = pedidoDto.ToModel();
        pedido.AssociarCliente(cliente);

        // Validação
        ValidarPedido(pedido);

        // Persistência
        _pedidoRepository.CriarPedido(pedido); // salva Pedido e seus Itens (por cascade ou explicitamente)
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

    public void Remover(int id)
    {
        Pedido pedido = RecuperarPedido(id);
        this._pedidoRepository.Remover(pedido);
    }

    private Pedido RecuperarPedido(int id)
    {
        return this._pedidoRepository.ObterPorId(id).GetAwaiter().GetResult()
           ?? throw new ServiceException($"Pedido não encontrado.");
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
        if (Math.Abs(pedido.ValorTotal - somaItens) > 0.01m)
            throw new ServiceException("O valor total do pedido está inconsistente com a soma dos subtotais dos itens.");
    }
}
