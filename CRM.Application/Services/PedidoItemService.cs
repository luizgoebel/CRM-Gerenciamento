using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
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

        PedidoItem item = dto.ToModel();

        Validar(item);

        this._pedidoItemRepository.Adicionar(item);
    }

    public void Atualizar(PedidoItemDto dto)
    {
        PedidoItem item = this._pedidoItemRepository.ObterPorId((int)dto.Id).GetAwaiter().GetResult()
            ?? throw new ServiceException("Item não encontrado.");

        item.Alterar((int)dto.Id, dto.ProdutoId, dto.Quantidade, dto.PrecoUnitario);

        this._pedidoItemRepository.Atualizar(item);
    }

    public async Task<PedidoItemDto> ObterPorId(int id)
    {
        PedidoItem item = await this._pedidoItemRepository.ObterPorId(id)
            ?? throw new ServiceException("Item não encontrado.");

        PedidoItemDto pedidoItemDto = item.ToDto();

        return pedidoItemDto;
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

        return itens.Select(item => item.ToDto());
    }

    private void Validar(PedidoItem pedidoItem)
    {
        ValidationResult resultado = pedidoItem.Validar();

        if (!resultado.IsValid)
            throw new DomainException(resultado.Erros.First());
    }
}
