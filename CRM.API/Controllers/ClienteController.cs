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
        _clienteService = clienteService;
    }

    [HttpGet("[action]")]
    public async Task<List<ClienteDto>> ObterTodosClientes()
    {
        List<ClienteDto> clientes = await _clienteService.ObterTodosClientes();
        return clientes;
    }
    [HttpGet("[action]")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        ClienteDto clienteDto = await _clienteService.ObterPorId(id);
        return Ok(clienteDto);
    }

    [HttpPost("[action]")]
    public IActionResult SalvarCliente([FromBody] ClienteDto clienteDto)
    {
        this._clienteService.Salvar(clienteDto);
        return Ok();
    }
}