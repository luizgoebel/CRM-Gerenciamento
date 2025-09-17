using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Services;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace CRM.Tests.Servicos;

[TestFixture]
public class PedidoServiceTests
{
    private Mock<IPedidoRepository> _pedidoRepositoryMock = null!;
    private Mock<IClienteRepository> _clienteRepositoryMock = null!;
    private Mock<IProdutoRepository> _produtoRepositoryMock = null!;
    private IPedidoService _pedidoService = null!;

    [SetUp]
    public void SetUp()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _pedidoService = new PedidoService(_pedidoRepositoryMock.Object, _clienteRepositoryMock.Object, _produtoRepositoryMock.Object);
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

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

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

    [Test]
    public async Task CriarPedido_QuandoOPedidoExiste_DeveAtualizarValoresDoPedido()
    {
        // Arrange  
        var pedidoDto = new PedidoDto
        {
            Id = 2,
            ClienteId = 1,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 5 }
            }
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

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

        _pedidoRepositoryMock.Setup(r => r.ObterPorId(2)).ReturnsAsync(new Pedido
        {
            Id = 2,
            ClienteId = 1,
            Itens = new List<PedidoItem>
            {
                new PedidoItem { ProdutoId = 1, Quantidade = 2, Produto = new Produto { Id = 1, Nome = "Produto Teste", Preco = 10m } }
            }
        });

        // Act  
        _pedidoService.CriarPedido(pedidoDto);           

        // Assert  
        _pedidoRepositoryMock.Verify(r => r.Atualizar(It.Is<Pedido>(p => p.Id == 2 && p.Itens.Any(i => i.Quantidade == 5))), Times.Once);
    }

    [Test]
    public async Task CriarPedido_ChamaOMetodoAualizarPedido_QuandoOPedidoEstaVazio_DeveAdicionarItem()
    {
        // Arrange  
        var pedidoDto = new PedidoDto
        {
            Id = 2,
            ClienteId = 1,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 5 }
            }
        };

        var pedidoExistente = new Pedido
        {
            Id = 2,
            ClienteId = 1,
            Itens = new List<PedidoItem>
            {
               new PedidoItem { }
            }
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

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

        _pedidoRepositoryMock.Setup(r => r.ObterPorId(2)).ReturnsAsync(pedidoExistente);

        // Act  
        _pedidoService.CriarPedido(pedidoDto);

        // Assert  
        _pedidoRepositoryMock.Verify(r => r.Atualizar(It.Is<Pedido>(p => p.Id == 2 && p.Itens.Any(i => i.Quantidade == 5))), Times.Once);
    }

    [Test]
    public async Task ObterPedidosPaginados_ComFiltro_DeveRetornarSomentePedidosFiltrados()
    {
        // Arrange
        var pedidos = new List<Pedido>
        {
            new Pedido { Id = 1, ClienteId = 1, Cliente = new Cliente { Id = 1, Nome = "João" }, DataCriacao = DateTime.Now },
            new Pedido { Id = 2, ClienteId = 2, Cliente = new Cliente { Id = 2, Nome = "Maria" }, DataCriacao = DateTime.Now }
        };

        _pedidoRepositoryMock.Setup(r => r.ObterQueryPedidos()).ReturnsAsync(pedidos.AsQueryable());

        // Act
        var resultado = await _pedidoService.ObterPedidosPaginados("maria", 1, 10);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, resultado.Total);
        Assert.AreEqual("Maria", resultado.Itens[0].Cliente);
    }

    [Test]
    public async Task ObterPedidosPaginados_SemFiltro_DeveRetornarTodosOsPedidos()
    {
        // Arrange
        var pedidos = new List<Pedido>
        {
            new Pedido { Id = 1, ClienteId = 1, Cliente = new Cliente { Id = 1, Nome = "João" }, DataCriacao = DateTime.Now },
            new Pedido { Id = 2, ClienteId = 2, Cliente = new Cliente { Id = 2, Nome = "Maria" }, DataCriacao = DateTime.Now }
        };

        _pedidoRepositoryMock.Setup(r => r.ObterQueryPedidos()).ReturnsAsync(pedidos.AsQueryable());

        // Act
        var resultado = await _pedidoService.ObterPedidosPaginados("", 1, 10);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Total);
        Assert.AreEqual("Maria", resultado.Itens[0].Cliente);
        Assert.AreEqual("João", resultado.Itens[1].Cliente);
    }

    [Test]
    public async Task ObterPorId_QuandoPedidoExiste_DeveRetornarPedido()
    {
        // Arrange  
        var pedido = new Pedido
        {
            Id = 2,
            ClienteId = 1,
            Itens = new List<PedidoItem>
            {
                new PedidoItem { ProdutoId = 1, Quantidade = 2 }
            }
        };
        _pedidoRepositoryMock.Setup(r => r.ObterPorId(2)).ReturnsAsync(pedido);

        // Act  
        var resultado = _pedidoService.ObterPorId(2).Result;

        // Assert  
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Id);
    }

    [Test]
    public async Task ObterTotalPedidosNaData_DeveRetornarTotalPedidos()
    {
        // Arrange  
        var data = new DateOnly(2023, 10, 1);
        _pedidoRepositoryMock.Setup(r => r.ObterTotalPedidosNaData(data)).ReturnsAsync(5);

        // Act  
        var total = await _pedidoService.ObterTotalPedidosNaData(data);

        // Assert  
        Assert.AreEqual(5, total);
    }

    [Test]
    public void RemoverPedido_QuandoPedidoEncontrado_ChamarRepositorio()
    {
        // Arrange  
        var pedido = new Pedido { Id = 1, ClienteId = 1 };
        _pedidoRepositoryMock.Setup(r => r.ObterPorId(1)).ReturnsAsync(pedido);
        _pedidoRepositoryMock.Setup(r => r.Remover(It.IsAny<Pedido>())).Verifiable();

        // Act  
        _pedidoService.Remover(1);

        // Assert  
        _pedidoRepositoryMock.Verify(r => r.Remover(It.Is<Pedido>(p => p.Id == 1)), Times.Once);
    }

    [Test]
    public async Task CriarPedido_ChamaOMetodoAtualizar_QuandoProdutoForNulo_DeveLancarExcessao()
    {
        // Arrange  
        var pedidoDto = new PedidoDto
        {
            Id = 2,
            ClienteId = 1,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 5 }
            }
        };

        var pedidoExistente = new Pedido
        {
            Id = 2,
            ClienteId = 1,
            Itens = new List<PedidoItem>
            {
                new PedidoItem { ProdutoId = 1, Quantidade = 2, Produto = null }
            }
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

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

        _pedidoRepositoryMock.Setup(r => r.ObterPorId(2)).ReturnsAsync(pedidoExistente);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _pedidoService.CriarPedido(pedidoDto));        
    }

    [Test]
    public void CriarPedido_QuandoOsItensDoPedidoForNulo_DeveLancarExcecao()
    {
        // Arrange
        var pedidoDto = new PedidoDto
        {
            Id = 0,
            ClienteId = 2,
            Itens = null
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

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

        // Act & Assert  
        Assert.Throws<ServiceException>(() => _pedidoService.CriarPedido(pedidoDto));
    }

    [Test]
    public void CriarPedido_QuandoOsItensDoPedidoForVazio_DeveLancarExcecao()
    {
        // Arrange
        var pedidoDto = new PedidoDto
        {
            Id = 0,
            ClienteId = 2,
            Itens = new List<PedidoItemDto>()
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

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

        // Act & Assert  
        Assert.Throws<ServiceException>(() => _pedidoService.CriarPedido(pedidoDto));
    }

    [Test]
    public void CriarPedido_QuandoTiverItensDuplicados_DeveLancarExcecao()
    {
        // Arrange
        var pedidoDto = new PedidoDto
        {
            Id = 0,
            ClienteId = 2,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 2 },
                new PedidoItemDto { ProdutoId = 1, Quantidade = 3 } 
            }
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 0,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

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

        // Act & Assert  
        Assert.Throws<ServiceException>(() => _pedidoService.CriarPedido(pedidoDto));
    }

    [Test]
    public void CriarPedido_QuandoCadastroForIncompleto_DeveLancarExcecao()
    {
        // Arrange
        var pedidoDto = new PedidoDto
        {
            Id = 0,
            ClienteId = 2,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 2 }
            }
        };

        _produtoRepositoryMock.Setup(r => r.ListarTodos())
            .Returns(new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Produto Teste",
                    Preco = 10m
                }
            });

        _clienteRepositoryMock.Setup(r => r.ObterPorId((int)pedidoDto.ClienteId))
            .ReturnsAsync(new Cliente
            {
                Id = (int)pedidoDto.ClienteId,
                Nome = "João",
                Telefone = "00000"
            });

        // Act & Assert  
        Assert.Throws<ServiceException>(() => _pedidoService.CriarPedido(pedidoDto));
    }
   
    [Test]
    public void CriarPedido_QuandoClienteIdForNegativo_DeveLancarExcecao()
    {
        // Arrange  
        var pedidoDto = new PedidoDto
        {
            Id = 0,
            ClienteId = -1,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 2 }
            }
        };

        // Act & Assert  
        Assert.Throws<ServiceException>(() => _pedidoService.CriarPedido(pedidoDto));
    }

    [Test]
    public void CriarPedido_ClienteNaoRegistrado_DeveLancarExcecao()
    {
        // Arrange  
        var pedidoDto = new PedidoDto
        {
            Id = 0,
            ClienteId = 0,
            Itens = new List<PedidoItemDto>
            {
                new PedidoItemDto { ProdutoId = 1, Quantidade = 2 }
            }
        };

        // Act & Assert  
        Assert.Throws<ServiceException>(() => _pedidoService.CriarPedido(pedidoDto));
    }

    [Test]
    public void CriarPedido_QuandoDtoForNulo_DeveLancarExcecao()
    {
        Assert.Throws<ServiceException>(() => _pedidoService.CriarPedido(null));
    }

    [Test]
    public void ObterPorId_QuandoNaoEncontrado_DeveLancarExcecao()
    {
        // Arrange  
        _pedidoRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((Pedido?)null);

        // Act & Assert  
        Assert.ThrowsAsync<ServiceException>(() => _pedidoService.ObterPorId(999));
    }

    [Test]
    public void RemoverPedido_QuandoPedidoNaoEncontrado_DeveLancarExcecaoQ()
    {
        // Arrange  
        _pedidoRepositoryMock
            .Setup(r => r.ObterPorId(It.IsAny<int>()))
            .ReturnsAsync((Pedido?)null);

        // Act & Assert  
        Assert.Throws<ServiceException>(() => _pedidoService.Remover(999));
    }

}
