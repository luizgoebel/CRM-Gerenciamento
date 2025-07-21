using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidoController(IPedidoService pedidoService)
    {
        this._pedidoService = pedidoService;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        PedidoDto pedidoDto = await this._pedidoService.ObterPorId(id);
        return Ok(pedidoDto);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterPedidosPaginados(int page, int pageSize, string filtro = "")
    {
        PaginacaoResultado<PedidoDto> resultado =
            await this._pedidoService.ObterPedidosPaginados(filtro, page, pageSize);
        return Ok(resultado);
    }

    [HttpPost("[action]")]
    public IActionResult CriarPedido([FromBody] PedidoDto pedidoDto)
    {
        this._pedidoService.CriarPedido(pedidoDto);
        return Ok();
    }
    [HttpPost("[action]")]
    public IActionResult Remover(int id)
    {
        this._pedidoService.Remover(id);
        return Ok();
    }
}
