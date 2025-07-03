using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        this._produtoRepository = produtoRepository;
    }

    public List<ProdutoDto> ListarTodos()
    {
        IEnumerable<Produto> produtos = [];
        produtos = this._produtoRepository.ListarTodos();

        return produtos.Select(p => p.ToDto()).ToList();
    }

    public async Task<PaginacaoResultado<ProdutoDto>> ObterProdutosPaginados(string filtro, int page, int pageSize)
    {
        IQueryable<Produto> query = await _produtoRepository.ObterQueryProdutos();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();
            query = query.Where(c => c.Nome.ToLower().Contains(filtro));
        }

        // Ordenar pela DataCriacao do mais recente para o mais antigo, depois pelo Nome
        query = query
            .OrderByDescending(c => c.DataCriacao)
            .ThenBy(c => c.Nome);

        if (!query.Any())
            return new PaginacaoResultado<ProdutoDto>();

        int total = query.Count();
        List<Produto> produtos = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        List<ProdutoDto> produtosDto = produtos.Select(p => p.ToDto()).ToList();

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
        Produto produto = RecuperarProduto(id);
        ProdutoDto produtoDto = produto.ToDto();

        return produtoDto;
    }

    public void Salvar(ProdutoDto produtoDto)
    {
        if (produtoDto == null)
            throw new NullReferenceException("O DTO do produto não pode ser nulo.");

        Validar(produtoDto.ToModel());

        if (produtoDto.Id > 0)
        {
            Produto produto = RecuperarProduto(produtoDto.Id ?? 0);
            produto.Alterar(produtoDto.Nome, produtoDto.Preco);
            this._produtoRepository.Atualizar(produto);

            return;
        }

        this._produtoRepository.Adicionar(produtoDto.ToModel());
    }

    public void Remover(int id)
    {
        Produto produto = RecuperarProduto(id);
        this._produtoRepository.Remover(produto);
    }

    private void Validar(Produto produto)
    {
        ValidationResult resultado = produto.Validar();

        if (!resultado.IsValid)
            throw new DomainException(resultado.Erros.First());
    }

    private Produto RecuperarProduto(int id)
    {
        return this._produtoRepository.ObterPorId(id).GetAwaiter().GetResult()
            ?? throw new ServiceException($"Produto não encontrado.");
    }
}
