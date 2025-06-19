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

        if (!cliente.CadastroCompleto())
            throw new ServiceException("Cadastro incompleto. Atualize o cadastro do cliente para prosseguir com o pedido.");

        Pedido pedido = new()
        {
            ClienteId = pedidoDto.ClienteId,
            Itens = pedidoDto.Itens.Select(i => new PedidoItem
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList()
        };
        this._pedidoRepository.CriarPedido(pedido);
    }

    public async Task<PedidoDto> ObterPorId(int id)
    {
            Pedido pedido = await this._pedidoRepository.ObterPorId(id)
            ?? throw new ServiceException($"Pedido não encontrado.");

            return new PedidoDto
            {
                ClienteId = pedido.ClienteId,
                Id = pedido.Id,
                Itens = pedido.Itens.Select(i => new PedidoItemDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };
    }
}