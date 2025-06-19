using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Services;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Servicos;

[TestFixture]
public class ProdutoServiceTests
{
    private Mock<IProdutoRepository> _produtoRepositoryMock = null!;
    private IProdutoService _produtoService = null!;

    [SetUp]
    public void SetUp()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _produtoService = new ProdutoService(_produtoRepositoryMock.Object);
    }

    [Test]
    public async Task Adicionar_DeveChamarRepositorioComProdutoCorreto()
    {
        var produtoDto = new ProdutoDto { Nome = "Produto A", Preco = 10.5m };

        _produtoRepositoryMock
            .Setup(r => r.Adicionar(It.IsAny<Produto>()))
            .Verifiable();

         _produtoService.Adicionar(produtoDto);

        _produtoRepositoryMock.Verify(r => r.Adicionar(It.Is<Produto>(p =>
            p.Nome == produtoDto.Nome && p.Preco == produtoDto.Preco)), Times.Once);
    }

    [Test]
    public async Task ListarTodos_DeveRetornarListaDeProdutoDto()
    {
        var produtos = new List<Produto>
        {
            new Produto { Id = 1, Nome = "Produto A", Preco = 10 },
            new Produto { Id = 2, Nome = "Produto B", Preco = 20 }
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos()).ReturnsAsync(produtos);

        var resultado = await _produtoService.ListarTodos();

        Assert.IsNotNull(resultado);
        Assert.That(resultado.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task ObterPorId_QuandoProdutoExiste_DeveRetornarProdutoDto()
    {
        var produto = new Produto { Id = 1, Nome = "Produto A", Preco = 10 };

        _produtoRepositoryMock.Setup(r => r.ObterPorId(1)).ReturnsAsync(produto);

        var resultado = await _produtoService.ObterPorId(1);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(produto.Id, resultado.Id);
        Assert.AreEqual(produto.Nome, resultado.Nome);
    }

    [Test]
    public void ObterPorId_QuandoProdutoNaoExiste_DeveLancarServiceException()
    {
        _produtoRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((Produto?)null);

        Assert.ThrowsAsync<ServiceException>(async () => await _produtoService.ObterPorId(999));
    }

    [Test]
    public async Task Atualizar_QuandoProdutoExiste_DeveChamarRepositorio()
    {
        var produtoDto = new ProdutoDto { Id = 1, Nome = "Produto Atualizado", Preco = 15 };

        _produtoRepositoryMock.Setup(r => r.ObterPorId(1))
            .ReturnsAsync(new Produto { Id = 1, Nome = "Produto Antigo", Preco = 10 });

        _produtoRepositoryMock.Setup(r => r.Atualizar(It.IsAny<Produto>()))
            .Verifiable();

        _produtoService.Atualizar(produtoDto);

        _produtoRepositoryMock.Verify(r => r.Atualizar(It.Is<Produto>(p =>
            p.Id == 1 && p.Nome == produtoDto.Nome && p.Preco == produtoDto.Preco)), Times.Once);
    }

    [Test]
    public void Atualizar_QuandoProdutoNaoExiste_DeveLancarServiceException()
    {
        _produtoRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((Produto?)null);

        var produtoDto = new ProdutoDto { Nome = "Produto", Preco = 10 };

        Assert.Throws<ServiceException>(() => _produtoService.Atualizar(produtoDto));
    }

    [Test]
    public async Task Remover_QuandoProdutoExiste_DeveChamarRepositorio()
    {
        var produto = new Produto { Id = 1, Nome = "Produto A", Preco = 10 };

        _produtoRepositoryMock.Setup(r => r.ObterPorId(1))
            .ReturnsAsync(produto);

        _produtoRepositoryMock.Setup(r => r.Remover(It.IsAny<Produto>()))
            .Verifiable();

         _produtoService.Remover(1);

        _produtoRepositoryMock.Verify(r => r.Remover(It.Is<Produto>(p => p.Id == 1)), Times.Once);
    }

    [Test]
    public void Remover_QuandoProdutoNaoExiste_DeveLancarServiceException()
    {
        _produtoRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((Produto?)null);

        Assert.Throws<ServiceException>(() => _produtoService.Remover(999));
    }
}
