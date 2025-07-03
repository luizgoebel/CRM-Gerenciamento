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
    public async Task CriarPedido_DevePermitirObterPedidoPorId()
    {
        // Arrange  
        var pedidoDto = new PedidoDto
        {
            Id = 0,
            ClienteId = 1,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 2 }
            }
        };

        _clienteRepositoryMock.Setup(r => r.ObterPorId((int)pedidoDto.ClienteId))
            .ReturnsAsync(new Cliente
            {
                Id = (int)pedidoDto.ClienteId,
                Nome = "João",
                Email = "joao@email.com",
                Endereco = "endereço",
                Telefone = "00000",
                DataCriacao = DateTime.Now,
                DataModificacao = DateTime.Now
            });

        _pedidoRepositoryMock
            .Setup(r => r.CriarPedido(It.IsAny<Pedido>()))
            .Callback<Pedido>(p => p.Id = 1);

        _pedidoRepositoryMock
            .Setup(r => r.ObterPorId(1))
            .ReturnsAsync(new Pedido
            {
                Id = 1,
                ClienteId = (int)pedidoDto.ClienteId,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem { ProdutoId = 1, Quantidade = 2 }
                }
            });

        // Act  
        _pedidoService.CriarPedido(pedidoDto);
        var teste = await _pedidoService.ObterPorId(1);

        // Assert  
        Assert.AreEqual(1, teste.Id);
    }

}
