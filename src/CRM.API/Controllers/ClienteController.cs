using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService clienteService)
    {
        this._clienteService = clienteService;
    }

    [HttpGet("[action]")]
    public async Task<List<ClienteDto>> ObterTodosClientes()
    {
        List<ClienteDto> clientes = await this._clienteService.ObterTodosClientes();
        return clientes;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterClientesPaginados(int page, int pageSize, string filtro = "")
    {
        PaginacaoResultado<ClienteDto> resultado =
            await this._clienteService.ObterClientesPaginados(filtro, page, pageSize);
        return Ok(resultado);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        ClienteDto clienteDto = await this._clienteService.ObterPorId(id);
        return Ok(clienteDto);
    }

    [HttpPost("[action]")]
    public IActionResult SalvarCliente([FromBody] ClienteDto clienteDto)
    {
        this._clienteService.Salvar(clienteDto);
        return Ok();
    }
}