using CRM.Domain.Entidades;

namespace CRM.Core.Interfaces;

public interface IClienteRepository
{
    Task<int> Adicionar(Cliente cliente);
    Task<Cliente?> ObterPorId(int id);
    Task Atualizar(Cliente cliente);
}
