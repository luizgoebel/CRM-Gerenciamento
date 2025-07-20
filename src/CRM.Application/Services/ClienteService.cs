using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class ClienteService(IClienteRepository clienteRepository) : IClienteService
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;

    public async Task<ClienteDto> ObterPorId(int id)
    {
        var cliente = await _clienteRepository.ObterPorId(id)
            ?? throw new ServiceException("Cliente não encontrado.");

        return cliente.ToDto();
    }

    public void Salvar(ClienteDto dto)
    {
        if (dto == null)
            throw new ServiceException("Dados do cliente inválidos.");

        var cliente = dto.ToModel();
        ValidarCadastroParcial(cliente);

        if (dto.Id > 0)
        {
            var existente = _clienteRepository.ObterPorId(dto.Id!.Value).GetAwaiter().GetResult()
                ?? throw new ServiceException("Cliente não encontrado para atualização.");

            existente.Alterar(dto.Nome, dto.Telefone, dto.Email, dto.Endereco);
            _clienteRepository.Atualizar(existente);
            return;
        }

        _clienteRepository.Adicionar(cliente);
    }

    public async Task<List<ClienteDto>> ObterTodosClientes()
    {
        var clientes = await _clienteRepository.ObterTodosClientes();

        if (clientes == null || !clientes.Any())
            return [];

        return clientes.Select(c => c.ToDto()).ToList();
    }

    public async Task<PaginacaoResultado<ClienteDto>> ObterClientesPaginados(string filtro, int page, int pageSize)
    {
        var query = await _clienteRepository.ObterQueryClientes();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.ToLower();
            query = query.Where(c => c.Nome.ToLower().Contains(texto));
        }

        query = query
            .OrderByDescending(c => c.DataCriacao)
            .ThenBy(c => c.Nome);

        if (!query.Any())
            return new();

        var total = query.Count();

        var clientes = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var clientesDto = clientes.Select(c => c.ToDto()).ToList();

        return new PaginacaoResultado<ClienteDto>
        {
            Itens = clientesDto,
            Total = total,
            PaginaAtual = page,
            TotalPaginas = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    private void ValidarCadastroParcial(Cliente cliente)
    {
        var resultado = cliente.ValidarCadastroParcial();

        if (!resultado.IsValid)
            throw new DomainException(resultado.Erros.First());
    }

    public async Task<int> ObterTotalClientes()
    {
        return await _clienteRepository.ObterTotalClientes();
    }
}
