using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
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
        
        Pedido pedido = new()
        {
            ClienteId = pedidoDto.ClienteId,
            Itens = pedidoDto.Itens.Select(i => new PedidoItem
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                Subtotal = i.PrecoUnitario * i.Quantidade
            }).ToList(),
            // Total do pedido = soma dos preços dos itens vezes q quantidade
            ValorTotal = pedidoDto.Itens.Sum(i => i.PrecoUnitario * i.Quantidade)
        };

        ValidarPedido(pedido);
        this._pedidoRepository.CriarPedido(pedido);
    }

    public async Task<PedidoDto> ObterPorId(int id)
    {
        Pedido pedido = await this._pedidoRepository.ObterPorId(id)
        ?? throw new ServiceException($"Pedido não encontrado.");

        return new PedidoDto
        {
            Id = pedido.Id,
            ClienteId = pedido.ClienteId,
            Itens = pedido.Itens.Select(i => new PedidoItemDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.Subtotal / (i.Quantidade == 0 ? 1 : i.Quantidade)
            }).ToList()
        };
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

        if (!pedido.Cliente.CadastroCompleto())
            throw new DomainException("Cadastro incompleto. Atualize o cadastro do cliente para prosseguir com o pedido.");

        decimal somaItens = pedido.Itens.Sum(i => i.Subtotal);
        if (pedido.ValorTotal != somaItens)
            throw new ServiceException("O valor total do pedido está inconsistente com a soma dos subtotais dos itens.");
    }
}
