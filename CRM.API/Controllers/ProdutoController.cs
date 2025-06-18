using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar([FromBody] ProdutoDto produtoDto)
        {
            await _produtoService.Adicionar(produtoDto);
            return CreatedAtAction(string.Empty, produtoDto);
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var produtos = await _produtoService.ListarTodos();
            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _produtoService.ObterPorId(id);
            return Ok(produto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoDto produtoDto)
        {
            await _produtoService.Atualizar(id, produtoDto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Remover(int id)
        {
            await _produtoService.Remover(id);
            return NoContent();
        }
    }
}
