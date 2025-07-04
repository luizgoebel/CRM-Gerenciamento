using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class ProdutoService(IProdutoRepository produtoRepository) : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository = produtoRepository;

    public List<ProdutoDto> ListarTodos()
    {
        var produtos = _produtoRepository.ListarTodos();
        return produtos.Select(p => p.ToDto()).ToList();
    }

    public async Task<PaginacaoResultado<ProdutoDto>> ObterProdutosPaginados(string filtro, int page, int pageSize)
    {
        var query = await _produtoRepository.ObterQueryProdutos();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.ToLower();
            query = query.Where(c => c.Nome.ToLower().Contains(texto));
        }

        query = query
            .OrderByDescending(c => c.DataCriacao)
            .ThenBy(c => c.Nome);

        if (!query.Any())
            return new();

        var total = query.Count();

        var produtos = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var produtosDto = produtos.Select(p => p.ToDto()).ToList();

        return new PaginacaoResultado<ProdutoDto>
        {
            Itens = produtosDto,
            Total = total,
            PaginaAtual = page,
            TotalPaginas = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    public ProdutoDto ObterPorId(int id)
    {
        var produto = _produtoRepository.ObterPorId(id).GetAwaiter().GetResult()
            ?? throw new ServiceException("Produto não encontrado.");

        return produto.ToDto();
    }

    public void Salvar(ProdutoDto dto)
    {
        if (dto == null)
            throw new ServiceException("Dados do produto inválidos.");

        var produto = dto.ToModel();
        Validar(produto);

        if (dto.Id > 0)
        {
            var existente = _produtoRepository.ObterPorId(dto.Id.Value).GetAwaiter().GetResult()
                ?? throw new ServiceException("Produto não encontrado para atualização.");

            existente.Alterar(dto.Nome, dto.Preco);
            _produtoRepository.Atualizar(existente);
            return;
        }

        _produtoRepository.Adicionar(produto);
    }

    public void Remover(int id)
    {
        var produto = _produtoRepository.ObterPorId(id).GetAwaiter().GetResult()
            ?? throw new ServiceException("Produto não encontrado.");

        _produtoRepository.Remover(produto);
    }

    private void Validar(Produto produto)
    {
        var resultado = produto.Validar();

        if (!resultado.IsValid)
            throw new DomainException(resultado.Erros.First());
    }

    public async Task<int> ObterTotalProdutos()
    {
        return await _produtoRepository.ObterTotalProdutos();
    }
}
