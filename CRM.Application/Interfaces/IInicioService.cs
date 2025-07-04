using CRM.Application.DTOs;

namespace CRM.Application.Interfaces;

public interface IInicioService
{
    Task<DashboardDto> ObterDadosDashboard();
}