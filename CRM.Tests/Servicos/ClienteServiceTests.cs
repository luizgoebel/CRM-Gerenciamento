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
    public void Adicionar_DeveRetornarIdDoClienteCriado()
    {
        // Arrange
        var clienteDto = new ClienteDto { Nome = "João", Email = "joao@email.com" };
        var cliente = new Cliente { Id = 1, Nome = clienteDto.Nome, Email = clienteDto.Email };

        _clienteRepositoryMock
            .Setup(r => r.Adicionar(It.IsAny<Cliente>()))
            .Callback<Cliente>(c => c.Id = cliente.Id);

        _clienteRepositoryMock
            .Setup(r => r.ObterPorId(cliente.Id))
            .ReturnsAsync(cliente);

        // Act
        _clienteService.Adicionar(clienteDto);
        var clienteRecuperado = _clienteRepositoryMock.Object.ObterPorId(cliente.Id).Result;

        // Assert
        Assert.IsNotNull(clienteRecuperado);
        Assert.AreEqual(cliente.Id, clienteRecuperado.Id);
    }

    [Test]
    public void Adicionar_QuandoDtoForNulo_DeveLancarServiceException()
    {
        Assert.Throws<ServiceException>(() => _clienteService.Adicionar(null));
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
        var clienteDto = new ClienteDto { Id = 1, Nome = "Carlos Atualizado", Email = "carlos@email.com" };

        _clienteRepositoryMock.Setup(r => r.ObterPorId(clienteExistente.Id))
            .ReturnsAsync(clienteExistente);

        _clienteRepositoryMock.Setup(r => r.Atualizar(clienteExistente));

        Assert.DoesNotThrow(() => _clienteService.Atualizar(clienteDto));
    }


    [Test]
    public void Atualizar_QuandoClienteNaoEncontrado_DeveLancarServiceException()
    {
        // Arrange
        var clienteDto = new ClienteDto { Id = 999, Nome = "Inexistente", Email = "inexistente@email.com" };
        _clienteRepositoryMock
            .Setup(r => r.ObterPorId(clienteDto.Id))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        var ex = Assert.Throws<ServiceException>(() =>
            _clienteService.Atualizar(clienteDto));

        Assert.That(ex.Message, Is.EqualTo("Cliente não encontrado."));
    }

}
