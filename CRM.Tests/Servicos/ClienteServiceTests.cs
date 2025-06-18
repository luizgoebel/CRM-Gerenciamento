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
public class ClienteServiceTests
{
    private Mock<IClienteRepository> _clienteRepositoryMock = null!;
    private IClienteService _clienteService = null!;

    [SetUp]
    public void SetUp()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _clienteService = new ClienteService(_clienteRepositoryMock.Object);
    }

    [Test]
    public async Task Adicionar_DeveRetornarIdDoClienteCriado()
    {
        // Arrange
        var clienteDto = new ClienteDto { Nome = "João", Email = "joao@email.com" };
        var cliente = new Cliente { Id = 1, Nome = clienteDto.Nome, Email = clienteDto.Email };

        _clienteRepositoryMock
            .Setup(r => r.Adicionar(It.IsAny<Cliente>()))
            .ReturnsAsync(cliente.Id);

        // Act
        var idRetornado = await _clienteService.Adicionar(clienteDto);

        // Assert
        Assert.AreEqual(cliente.Id, idRetornado);
    }

    [Test]
    public void Adicionar_QuandoDtoForNulo_DeveLancarServiceException()
    {
        Assert.ThrowsAsync<ServiceException>(async () => await _clienteService.Adicionar(null!));
    }

    [Test]
    public async Task ObterPorId_DeveRetornarClienteDto_QuandoEncontrado()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nome = "Maria", Email = "maria@email.com" };
        _clienteRepositoryMock
            .Setup(r => r.ObterPorId(cliente.Id))
            .ReturnsAsync(cliente);

        // Act
        var resultado = await _clienteService.ObterPorId(cliente.Id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(cliente.Id, resultado.Id);
        Assert.AreEqual(cliente.Nome, resultado.Nome);
    }

    [Test]
    public void ObterPorId_QuandoNaoEncontrado_DeveLancarServiceException()
    {
        _clienteRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((Cliente?)null);

        Assert.ThrowsAsync<ServiceException>(async () => await _clienteService.ObterPorId(999));
    }

    [Test]
    public async Task Atualizar_DeveExecutarSemExcecao_QuandoClienteExiste()
    {
        var clienteExistente = new Cliente { Id = 1, Nome = "Carlos", Email = "carlos@email.com" };
        var clienteDto = new ClienteDto { Nome = "Carlos Atualizado", Email = "carlos@email.com" };

        _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteExistente.Id)).ReturnsAsync(clienteExistente);
        _clienteRepositoryMock.Setup(r => r.Atualizar(clienteExistente)).Returns(Task.CompletedTask);

        Assert.DoesNotThrowAsync(async () => await _clienteService.Atualizar(clienteExistente.Id, clienteDto));
    }

    [Test]
    public void Atualizar_QuandoClienteNaoEncontrado_DeveLancarServiceException()
    {
        _clienteRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((Cliente?)null);

        Assert.ThrowsAsync<ServiceException>(async () => await _clienteService.Atualizar(999, new ClienteDto()));
    }
}
