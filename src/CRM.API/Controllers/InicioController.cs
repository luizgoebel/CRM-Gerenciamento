using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InicioController : ControllerBase
{
    private readonly IInicioService _inicioService;

    public InicioController(IInicioService inicioService)
    {
        this._inicioService = inicioService;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ObterDadosDashboard()
    {
        DashboardDto dashboard = await _inicioService.ObterDadosDashboard();
        return Ok(dashboard);
    }
}