using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;

    public PedidoService(IPedidoRepository pedidoRepository, IClienteRepository clienteRepository, IProdutoRepository produtoRepository)
    {
        this._pedidoRepository = pedidoRepository;
        this._clienteRepository = clienteRepository;
        this._produtoRepository = produtoRepository;
    }

   

    public async Task<PaginacaoResultado<PedidoDto>> ObterPedidosPaginados(string filtro, int page, int pageSize)
    {
        IQueryable<Pedido> query = await this._pedidoRepository.ObterQueryPedidos();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();
            query = query.Where(c => c.Cliente!.Nome.ToLower().Contains(filtro));
        }

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

    private void ValidarPedido(Pedido pedido, Cliente cliente)
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

        if (!cliente.CadastroCompleto())
            throw new ServiceException("Cadastro incompleto. Atualize o cadastro do cliente para prosseguir com o pedido.");

        decimal somaItens = pedido.Itens.Sum(i => i.Subtotal);
        if (Math.Abs(pedido.ValorTotal - somaItens) > 0.01m)
            throw new ServiceException("O valor total do pedido está inconsistente com a soma dos subtotais dos itens.");
    }
    public void CriarPedido(PedidoDto pedidoDto)
    {
        if (pedidoDto == null) throw new ServiceException("Verificar dados do pedido.");
        if (pedidoDto.ClienteId <= 0) throw new ServiceException("Não há cliente vinculado ao pedido.");

        bool pedidoExiste = pedidoDto.Id.HasValue && pedidoDto.Id > 0;
        if (pedidoExiste)
        {
            AtualizarPedidoExistente(pedidoDto);
        }
        else
        {
            CriarNovoPedido(pedidoDto);
        }
    }

    private void CriarNovoPedido(PedidoDto pedidoDto)
    {
        var cliente = _clienteRepository.ObterPorId((int)pedidoDto.ClienteId!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Cliente não encontrado.");

        var pedido = pedidoDto.ToModel();

        List<int> idsSelecionados = pedido.Itens.Select(i => i.ProdutoId).Distinct().ToList();
        List<Produto> produtos = _produtoRepository.ListarTodos().Where(x => idsSelecionados.Contains(x.Id)).ToList();

        foreach (PedidoItem pedidoItem in pedido.Itens)
        {
            Produto? produto = produtos.FirstOrDefault(x => x.Id == pedidoItem.ProdutoId);
            if (produto != null)
                pedidoItem.AtualizarValores(produto);
        }

        pedido.AtualizarValorTotal();

        ValidarPedido(pedido, cliente);

        _pedidoRepository.CriarPedido(pedido);
    }

    private void AtualizarPedidoExistente(PedidoDto pedidoDto)
    {
        Pedido pedidoExistente = _pedidoRepository.ObterPorId((int)pedidoDto.Id!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Pedido não encontrado para atualização.");

        var cliente = _clienteRepository.ObterPorId((int)pedidoDto.ClienteId!).GetAwaiter().GetResult()
            ?? throw new ServiceException("Cliente não encontrado.");

        // Mapeia os itens do DTO para um dicionário por ProdutoId
        var itensDto = pedidoDto.ToModel().Itens;
        var itensExistentes = pedidoExistente.Itens;

        foreach (var itemDto in itensDto)
        {
            var itemExistente = itensExistentes.FirstOrDefault(i => i.ProdutoId == itemDto.ProdutoId);
            if (itemExistente != null)
            {
                // Atualiza a quantidade e outros campos relevantes
                itemExistente.Quantidade = itemDto.Quantidade;
                itemExistente.AtualizarValores(itemExistente.Produto!);
            }
            else
            {
                // Adiciona novo item ao pedido
                itensExistentes.Add(itemDto);
            }
        }

        // Remove itens que não estão mais no DTO
        pedidoExistente.Itens = itensExistentes
            .Where(i => itensDto.Any(dto => dto.ProdutoId == i.ProdutoId))
            .ToList();

        // Atualiza valores dos produtos
        List<int> idsSelecionados = pedidoExistente.Itens.Select(i => i.ProdutoId).Distinct().ToList();
        List<Produto> produtos = _produtoRepository.ListarTodos().Where(x => idsSelecionados.Contains(x.Id)).ToList();

        foreach (PedidoItem pedidoItem in pedidoExistente.Itens)
        {
            Produto? produto = produtos.FirstOrDefault(x => x.Id == pedidoItem.ProdutoId);
            if (produto != null)
                pedidoItem.AtualizarValores(produto);
        }

        pedidoExistente.AtualizarValorTotal();

        ValidarPedido(pedidoExistente, cliente);

        // Lembre-se de implementar o método de atualização no repositório
        throw new NotImplementedException("Implementar método de atualização de pedido no repositório.");
    }
}