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
public class PedidoServiceTests
{
    private Mock<IPedidoRepository> _pedidoRepositoryMock = null!;
    private Mock<IClienteRepository> _clienteRepositoryMock = null!;
    private IPedidoService _pedidoService = null!;

    [SetUp]
    public void SetUp()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _pedidoService = new PedidoService(_pedidoRepositoryMock.Object, _clienteRepositoryMock.Object);
    }

    [Test]
    public async Task CriarPedido_DeveRetornarIdDoPedidoCriado()
    {
        // Arrange
        var pedidoDto = new PedidoDto
        {
            ClienteId = 1,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 2 }
            }
        };

        _clienteRepositoryMock.Setup(r => r.ObterPorId(pedidoDto.ClienteId))
            .ReturnsAsync(new Cliente { Id = pedidoDto.ClienteId, Nome = "João", Email = "joao@email.com", Endereco = "endereço", Telefone = "00000"});

        _pedidoRepositoryMock
            .Setup(r => r.CriarPedido(It.IsAny<Pedido>()))
            .ReturnsAsync(1);

        // Act
        var idRetornado = await _pedidoService.CriarPedido(pedidoDto);

        // Assert
        Assert.AreEqual(1, idRetornado);
    }

    [Test]
    public void CriarPedido_QuandoClienteNaoExistir_DeveLancarServiceException()
    {
        var pedidoDto = new PedidoDto { ClienteId = 999 };

        _clienteRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>()))
            .ReturnsAsync((Cliente?)null);

        Assert.ThrowsAsync<ServiceException>(async () => await _pedidoService.CriarPedido(pedidoDto));
    }

    [Test]
    public void CriarPedido_QuandoNaoHouverItens_DeveLancarServiceException()
    {
        var pedidoDto = new PedidoDto
        {
            ClienteId = 1,
            Itens = new List<PedidoItemDto>() // vazio
        };

        _clienteRepositoryMock.Setup(r => r.ObterPorId(pedidoDto.ClienteId))
            .ReturnsAsync(new Cliente { Id = pedidoDto.ClienteId, Nome = "João" });

        Assert.ThrowsAsync<ServiceException>(async () => await _pedidoService.CriarPedido(pedidoDto));
    }

    [Test]
    public async Task ObterPorId_DeveRetornarPedidoDto_QuandoEncontrado()
    {
        var pedido = new Pedido
        {
            Id = 1,
            ClienteId = 1,
            Itens = new List<PedidoItem>
            {
                new PedidoItem { ProdutoId = 1, Quantidade = 2 }
            }
        };

        _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedido.Id)).ReturnsAsync(pedido);

        var resultado = await _pedidoService.ObterPorId(pedido.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(pedido.Id, resultado.Id);
    }

    [Test]
    public void ObterPorId_QuandoNaoEncontrado_DeveLancarServiceException()
    {
        _pedidoRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((Pedido?)null);

        Assert.ThrowsAsync<ServiceException>(async () => await _pedidoService.ObterPorId(999));
    }
}
