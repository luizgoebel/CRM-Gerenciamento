using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        this._produtoService = produtoService;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterTodosProdutos()
    {
        List<ProdutoDto> produtos = await this._produtoService.ListarTodos();
        return Ok(produtos);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterProdutosPaginados(string filtro, int page = 1, int pageSize = 25)
    {
        PaginacaoResultado<ProdutoDto> resultado = await this._produtoService.ObterProdutosPaginados(filtro, page, pageSize);
        return Ok(resultado);
    }

    [HttpGet("[action]")]
    public IActionResult ObterPorId(int id)
    {
        ProdutoDto produto = this._produtoService.ObterPorId(id);
        return Ok(produto);
    }

    [HttpPost("[action]")]
    public IActionResult Salvar([FromBody] ProdutoDto produtoDto)
    {
        this._produtoService.Salvar(produtoDto);
        return Ok();
    }

    [HttpPost("[action]")]
    public IActionResult Remover(int id)
    {
        this._produtoService.Remover(id);
        return this.Ok();
    }
}
