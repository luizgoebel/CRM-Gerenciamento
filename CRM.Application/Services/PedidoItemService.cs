using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class PedidoItemService : IPedidoItemService
{
    private readonly IPedidoItemRepository _pedidoItemRepository;

    public PedidoItemService(IPedidoItemRepository pedidoItemRepository)
    {
        this._pedidoItemRepository = pedidoItemRepository;
    }

    public void Adicionar(PedidoItemDto dto)
    {
        if (dto == null)
            throw new ServiceException("Item inválido.");

        PedidoItem item = new PedidoItem
        {
            ProdutoId = dto.ProdutoId,
            Quantidade = dto.Quantidade,
            Subtotal = dto.Quantidade * dto.PrecoUnitario
        };

        Validar(item);

        this._pedidoItemRepository.Adicionar(item);
    }

    public void Atualizar(PedidoItemDto dto)
    {
        PedidoItem item = this._pedidoItemRepository.ObterPorId(dto.Id).GetAwaiter().GetResult()
            ?? throw new ServiceException("Item não encontrado.");

        var subtotal = dto.Quantidade * dto.PrecoUnitario;

        item.Alterar(dto.PedidoId, dto.ProdutoId, dto.Quantidade, subtotal);

        this._pedidoItemRepository.Atualizar(item);
    }

    public async Task<PedidoItemDto> ObterPorId(int id)
    {
        PedidoItem item = await _pedidoItemRepository.ObterPorId(id)
            ?? throw new ServiceException("Item não encontrado.");

        return new PedidoItemDto
        {
            Id = item.Id,
            PedidoId = item.PedidoId,
            ProdutoId = item.ProdutoId,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.Subtotal / item.Quantidade
        };
    }

    public void Remover(int id)
    {
        PedidoItem item = this._pedidoItemRepository.ObterPorId(id).GetAwaiter().GetResult()
            ?? throw new ServiceException("Item não encontrado.");

        this._pedidoItemRepository.Remover(item);
    }

    public async Task<IEnumerable<PedidoItemDto>> ListarPorPedido(int pedidoId)
    {
        var itens = await _pedidoItemRepository.ListarPorPedido(pedidoId)
            ?? throw new ServiceException("Nenhum item encontrado."); ;

        return itens.Select(item => new PedidoItemDto
        {
            Id = item.Id,
            PedidoId = item.PedidoId,
            ProdutoId = item.ProdutoId,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.Subtotal / item.Quantidade
        });
    }

    private void Validar(PedidoItem pedidoItem)
    {
        ValidationResult resultado = pedidoItem.Validar();

        if (!resultado.IsValid)
            throw new DomainException(resultado.Erros.First());
    }
}
