using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;   
        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var cliente = await _clienteService.ObterPorId(id);
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar([FromBody] ClienteDto clienteDto)
        {
            var novoId = await _clienteService.Adicionar(clienteDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = novoId }, clienteDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ClienteDto clienteDto)
        {
            await _clienteService.Atualizar(id, clienteDto);
            return NoContent();
        }
    }
}
