﻿using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Application.Mappers;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        this._clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto> ObterPorId(int id)
    {
        Cliente cliente = await this._clienteRepository.ObterPorId(id)
            ?? throw new ServiceException("Cliente não encontrado.");
        ClienteDto clienteDto = cliente.ToDto();

        return clienteDto;
    }

    public void Salvar(ClienteDto clienteDto)
    {
        if (clienteDto == null)
            throw new ServiceException("Ocorreu um erro.");
        ValidarCadastroParcial(clienteDto.ToModel());

        if (clienteDto.Id > 0)
        {
            Cliente cliente = this._clienteRepository.ObterPorId(clienteDto.Id ?? 0).GetAwaiter().GetResult()
                  ?? throw new ServiceException("Cliente não encontrado.");
            cliente.Alterar(clienteDto.Nome, clienteDto.Telefone, clienteDto.Email, clienteDto.Endereco);
            this._clienteRepository.Atualizar(cliente);

            return;
        }

        _clienteRepository.Adicionar(clienteDto.ToModel());
    }

    private void ValidarCadastroParcial(Cliente cliente)
    {
        ValidationResult resultado = cliente.ValidarCadastroParcial();

        if (!resultado.IsValid)
            throw new DomainException(resultado.Erros.First());
    }

    public async Task<List<ClienteDto>> ObterTodosClientes()
    {
        var clientes = await this._clienteRepository.ObterTodosClientes();
        if (clientes == null || !clientes.Any())
            return [];

        var clientesDto = clientes.Select(c => c.ToDto()).ToList();

        return clientesDto;
    }

    public async Task<PaginacaoResultado<ClienteDto>> ObterClientesPaginados(string filtro, int page, int pageSize)
    {
        IQueryable<Cliente> query = await this._clienteRepository.ObterQueryClientes();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();
            query = query.Where(c => c.Nome.ToLower().Contains(filtro));
        }

        if (!query.Any())
            return new PaginacaoResultado<ClienteDto>();

        int total = query.Count();
        List<Cliente> clientes = [.. query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)];

        List<ClienteDto> clientesDto = [.. clientes.Select(c => c.ToDto())];

        return new PaginacaoResultado<ClienteDto>
        {
            Itens = clientesDto,
            Total = total,
            PaginaAtual = page,
            TotalPaginas = (int)Math.Ceiling((double)total / pageSize)
        };
    }
}
