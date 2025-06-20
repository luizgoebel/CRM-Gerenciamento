using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        this._produtoService = produtoService;
    }

    [HttpPost("[action]")]
    public IActionResult Adicionar([FromBody] ProdutoDto produtoDto)
    {
        this._produtoService.Adicionar(produtoDto);
        return Ok();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ListarTodos()
    {
        IEnumerable<ProdutoDto> produtos = await this._produtoService.ListarTodos();
        return Ok(produtos);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        ProdutoDto produto = this._produtoService.ObterPorId(id);
        return Ok(produto);
    }

    [HttpPost("[action]")]
    public IActionResult Atualizar(int id, [FromBody] ProdutoDto produtoDto)
    {
        this._produtoService.Atualizar(produtoDto);
        return Ok();
    }

    [HttpPost("[action]")]
    public IActionResult Remover(int id)
    {
        this._produtoService.Remover(id);
        return this.Ok();
    }
}
