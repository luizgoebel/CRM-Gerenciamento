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
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<int> CriarPedido(PedidoDto pedidoDto)
    {
        var cliente = await _clienteRepository.ObterPorId(pedidoDto.ClienteId);
        if (cliente == null)
            throw new ServiceException($"Cliente com ID {pedidoDto.ClienteId} não encontrado.");

        if (string.IsNullOrEmpty(cliente.Telefone) || string.IsNullOrEmpty(cliente.Endereco))
            throw new ServiceException("Cadastro incompleto. Atualize o cadastro do cliente para prosseguir com o pedido.");

        var pedido = new Pedido
        {
            ClienteId = pedidoDto.ClienteId,
            Itens = pedidoDto.Itens.Select(i => new PedidoItem
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList()
        };

        return await _pedidoRepository.CriarPedido(pedido);
    }

    public async Task<PedidoDto> ObterPorId(int id)
    {
        var pedido = await _pedidoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Pedido com ID {id} não encontrado.");

        return new PedidoDto
        {
            ClienteId = pedido.ClienteId,
            Itens = pedido.Itens.Select(i => new PedidoItemDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList()
        };
    }
}