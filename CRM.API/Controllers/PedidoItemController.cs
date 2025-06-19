using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoItemController : ControllerBase
{
    private readonly IPedidoItemService _pedidoItemService;

    public PedidoItemController(IPedidoItemService pedidoItemService)
    {
        _pedidoItemService = pedidoItemService;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var item = await this._pedidoItemService.ObterPorId(id);
        return Ok(item);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ListarPorPedido(int pedidoId)
    {
        var itens = await this._pedidoItemService.ListarPorPedido(pedidoId);
        return Ok(itens);
    }

    [HttpPost("[action]")]
    public IActionResult Adicionar([FromBody] PedidoItemDto dto)
    {
        this._pedidoItemService.Adicionar(dto);
        return Ok();
    }

    [HttpPost("[action]")]
    public IActionResult Atualizar(int id, [FromBody] PedidoItemDto dto)
    {
        this._pedidoItemService.Atualizar(dto);
        return Ok();
    }

    [HttpPost("[action]")]
    public IActionResult Remover(int id)
    {
        this._pedidoItemService.Remover(id);
        return Ok();
    }
}
