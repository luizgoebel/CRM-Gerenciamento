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
    private readonly IProdutoRepository _produtoRepository;

    public PedidoItemService(IPedidoItemRepository pedidoItemRepository, IProdutoRepository produtoRepository)
    {
        this._pedidoItemRepository = pedidoItemRepository;
        this._produtoRepository = produtoRepository;
    }

    public void Adicionar(PedidoItemDto dto)
    {
        if (dto == null)
            throw new ServiceException("Ocorreu um erro ao processar alguns itens.");

        PedidoItem item = dto.ToModel();
        Produto? produto = _produtoRepository.ObterPorId(item.ProdutoId).GetAwaiter().GetResult() 
            ?? throw new ServiceException($"Ocorreu um erro ao processar o produto."); ;
        item.AtualizarValores(produto);
        Validar(item);
        this._pedidoItemRepository.Adicionar(item);
    }

    public void Atualizar(PedidoItemDto dto)
    {
        if (EhPedidoItemDtoValido(dto))
            throw new ServiceException("Item inválido.");

        PedidoItem item = this._pedidoItemRepository.ObterPorId((int)dto.Id!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Ocorreu um erro ao processar alguns itens.");
        Produto produto = this._produtoRepository.ObterPorId((int)item.ProdutoId!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Ocorreu um erro ao processar alguns itens.");

        item.Quantidade = dto.Quantidade ?? item.Quantidade;

        item.AtualizarValores(produto);

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
    private bool EhPedidoItemDtoValido(PedidoItemDto dto)
    {
        return dto != null
            && dto.Id.HasValue && dto.Id.Value > 0
            && dto.PedidoId.HasValue && dto.PedidoId.Value > 0
            && dto.ProdutoId.HasValue && dto.ProdutoId.Value > 0
            && dto.Quantidade.HasValue && dto.Quantidade.Value > 0;
    }
}
