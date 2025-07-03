using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Services;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Servicos
{
    [TestFixture]
    public class PedidoItemServiceTests
    {
        private Mock<IPedidoItemRepository> _pedidoItemRepositoryMock;
        private Mock<IProdutoRepository> _produtoRepositoryMock;
        private PedidoItemService _pedidoItemService;

        [SetUp]
        public void Setup()
        {
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _pedidoItemRepositoryMock = new Mock<IPedidoItemRepository>();
            _pedidoItemService = new PedidoItemService(_pedidoItemRepositoryMock.Object, _produtoRepositoryMock.Object);
        }

        [Test]
        public void Adicionar_DeveSalvarPedidoItem()
        {
            var produto = new Produto { Id = 2, Nome = "Produto Teste", Preco = 10 };
            _produtoRepositoryMock.Setup(r => r.ObterPorId(2)).ReturnsAsync(produto);

            var dto = new PedidoItemDto
            {
                Id = 1,
                ProdutoId = 2,
                Quantidade = 3,
                PrecoUnitario = 10,
            };

            PedidoItem pedidoItemCapturado = null;
            _pedidoItemRepositoryMock
                .Setup(r => r.Adicionar(It.IsAny<PedidoItem>()))
                .Callback<PedidoItem>(p => pedidoItemCapturado = p);

            _pedidoItemService.Adicionar(dto);

            _pedidoItemRepositoryMock.Verify(r => r.Adicionar(It.Is<PedidoItem>(p =>
                p.ProdutoId == dto.ProdutoId &&
                p.Quantidade == dto.Quantidade
            )), Times.Once);
        }


        [Test]
        public void Adicionar_QuandoDtoForNulo_DeveLancarExcecao()
        {
            Assert.Throws<ServiceException>(() => _pedidoItemService.Adicionar(null));
        }

        [Test]
        public async Task ObterPorId_QuandoItemExiste_DeveRetornarDto()
        {
            var item = new PedidoItem { Id = 1, PedidoId = 1, ProdutoId = 2, Quantidade = 3 };

            _pedidoItemRepositoryMock.Setup(r => r.ObterPorId(1)).ReturnsAsync(item);

            var resultado = await _pedidoItemService.ObterPorId(1);

            Assert.AreEqual(item.Id, resultado.Id);
            Assert.AreEqual(item.ProdutoId, resultado.ProdutoId);
            Assert.AreEqual(item.Quantidade, resultado.Quantidade);
            Assert.AreEqual(item.PrecoUnitario, resultado.PrecoUnitario);
        }

        [Test]
        public void ObterPorId_QuandoItemNaoExiste_DeveLancarExcecao()
        {
            _pedidoItemRepositoryMock.Setup(r => r.ObterPorId(1)).ReturnsAsync((PedidoItem?)null);

            Assert.ThrowsAsync<ServiceException>(() => _pedidoItemService.ObterPorId(1));
        }

        [Test]
        public void Atualizar_DeveAlterarItem()
        {
            var dto = new PedidoItemDto { Id = 1, PedidoId = 1, ProdutoId = 2, Quantidade = 1 };
            var itemExistente = new PedidoItem { Id = 1, PedidoId = 1, ProdutoId = 2, Quantidade = 2 };

            var produto = new Produto { Id = 2, Nome = "Produto Teste", Preco = 10 };
            _produtoRepositoryMock.Setup(r => r.ObterPorId(2)).ReturnsAsync(produto);

            _pedidoItemRepositoryMock.Setup(r => r.ObterPorId((int)dto.Id)).ReturnsAsync(itemExistente);

            _pedidoItemService.Atualizar(dto);

            _pedidoItemRepositoryMock.Verify(r => r.Atualizar(It.Is<PedidoItem>(p =>
                p.Id == dto.Id &&
                p.ProdutoId == dto.ProdutoId &&
                p.Quantidade == dto.Quantidade &&
                p.Subtotal == dto.Quantidade * produto.Preco // Corrigido: usa o preço do produto, não do DTO
            )), Times.Once);
        }

        [Test]
        public void Atualizar_QuandoNaoEncontrado_DeveLancarExcecao()
        {
            _pedidoItemRepositoryMock.Setup(r => r.ObterPorId(It.IsAny<int>())).ReturnsAsync((PedidoItem?)null);

            var dto = new PedidoItemDto { Id = 999, ProdutoId = 1, Quantidade = 1, PrecoUnitario = 1 };

            Assert.Throws<ServiceException>(() => _pedidoItemService.Atualizar(dto));
        }

        [Test]
        public async Task Remover_DeveChamarRepositorio()
        {
            var item = new PedidoItem { Id = 1 };
            _pedidoItemRepositoryMock.Setup(r => r.ObterPorId(1)).ReturnsAsync(item);

            _pedidoItemService.Remover(1);

            _pedidoItemRepositoryMock.Verify(r => r.Remover(item), Times.Once);
        }

        [Test]
        public void Remover_QuandoNaoEncontrado_DeveLancarExcecao()
        {
            _pedidoItemRepositoryMock.Setup(r => r.ObterPorId(1)).ReturnsAsync((PedidoItem?)null);

            Assert.Throws<ServiceException>(() => _pedidoItemService.Remover(1));
        }

        [Test]
        public async Task ListarPorPedido_DeveRetornarListaDeDtos()
        {
            var itens = new List<PedidoItem>
            {
                new PedidoItem { Id = 1, PedidoId = 1, ProdutoId = 1, Quantidade = 2 },
                new PedidoItem { Id = 2, PedidoId = 1, ProdutoId = 2, Quantidade = 1 }
            };

            _pedidoItemRepositoryMock.Setup(r => r.ListarPorPedido(1)).ReturnsAsync(itens);

            var resultado = await _pedidoItemService.ListarPorPedido(1);

            Assert.AreEqual(2, resultado.Count());
        }
    }
}
