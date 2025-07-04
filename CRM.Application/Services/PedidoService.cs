using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class PedidoService(IPedidoRepository pedidoRepository, IClienteRepository clienteRepository, IProdutoRepository produtoRepository) : IPedidoService
{
    private readonly IProdutoRepository _produtoRepository = produtoRepository;
    private readonly IPedidoRepository _pedidoRepository = pedidoRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<PaginacaoResultado<PedidoDto>> ObterPedidosPaginados(string filtro, int page, int pageSize)
    {
        var query = await _pedidoRepository.ObterQueryPedidos();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.ToLower();
            query = query.Where(c => c.Cliente!.Nome.ToLower().Contains(texto));
        }

        query = query.OrderByDescending(c => c.DataCriacao).ThenBy(c => c.Cliente!.Nome);

        if (!query.Any())
            return new();

        var total = query.Count();

        var pedidos = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var pedidosDto = pedidos.Select(c => c.ToDto()).ToList();

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
        var pedido = await _pedidoRepository.ObterPorId(id)
            ?? throw new ServiceException("Não foi possível localizar o pedido.");

        return pedido.ToDto();
    }

    public void Remover(int id)
    {
        var pedido = _pedidoRepository.ObterPorId(id).GetAwaiter().GetResult()
            ?? throw new ServiceException("Pedido não encontrado.");

        _pedidoRepository.Remover(pedido);
    }

    public void CriarPedido(PedidoDto pedidoDto)
    {
        if (pedidoDto == null || pedidoDto.ClienteId <= 0)
            throw new ServiceException("Pedido inválido. Verifique os dados informados.");

        if (pedidoDto.Id.HasValue && pedidoDto.Id > 0)
            AtualizarPedido(pedidoDto);
        else
            CriarPedidoNovo(pedidoDto);
    }

    private void CriarPedidoNovo(PedidoDto dto)
    {
        var cliente = _clienteRepository.ObterPorId((int)dto.ClienteId!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Cliente não encontrado.");

        var pedido = dto.ToModel();

        AtualizarItensComDadosProduto(pedido.Itens);
        pedido.AtualizarValorTotal();

        ValidarPedido(pedido, cliente);

        _pedidoRepository.CriarPedido(pedido);
    }

    private void AtualizarPedido(PedidoDto dto)
    {
        var pedido = _pedidoRepository.ObterPorId((int)dto.Id!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Pedido não encontrado para atualização.");

        var cliente = _clienteRepository.ObterPorId((int)dto.ClienteId!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Cliente não encontrado.");

        var itensDto = dto.ToModel().Itens;

        foreach (var itemDto in itensDto)
        {
            var existente = pedido.Itens.FirstOrDefault(i => i.ProdutoId == itemDto.ProdutoId);
            if (existente != null)
            {
                existente.Quantidade = itemDto.Quantidade;
                existente.AtualizarValores(existente.Produto!);
            }
            else
            {
                pedido.Itens.Add(itemDto);
            }
        }

        pedido.Itens = pedido.Itens
            .Where(i => itensDto.Any(dtoItem => dtoItem.ProdutoId == i.ProdutoId))
            .ToList();

        AtualizarItensComDadosProduto(pedido.Itens);
        pedido.AtualizarValorTotal();

        ValidarPedido(pedido, cliente);

        _pedidoRepository.Atualizar(pedido);
    }

    private void AtualizarItensComDadosProduto(List<PedidoItem> itens)
    {
        var ids = itens.Select(i => i.ProdutoId).Distinct().ToList();
        var produtos = _produtoRepository.ListarTodos().Where(p => ids.Contains(p.Id)).ToList();

        foreach (var item in itens)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == item.ProdutoId);
            if (produto != null)
                item.AtualizarValores(produto);
        }
    }

    private void ValidarPedido(Pedido pedido, Cliente cliente)
    {
        if (pedido.Itens == null || pedido.Itens.Count == 0)
            throw new ServiceException("O pedido deve conter pelo menos um item.");

        var duplicados = pedido.Itens
            .GroupBy(i => i.ProdutoId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicados.Any())
            throw new ServiceException("Há produtos duplicados no pedido.");

        if (!cliente.CadastroCompleto())
            throw new ServiceException("O cadastro do cliente está incompleto. Atualize para prosseguir.");

        var soma = pedido.Itens.Sum(i => i.Subtotal);
        if (Math.Abs(pedido.ValorTotal - soma) > 0.01m)
            throw new ServiceException("O valor total está incorreto com base nos itens informados.");
    }
}
