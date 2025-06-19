using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Core.Interfaces;
using CRM.Domain.Entidades;

namespace CRM.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public void Adicionar(ClienteDto clienteDto)
    {
        if (clienteDto == null)
            throw new ServiceException("Cliente inválido.");

        var cliente = new Cliente
            {
                Nome = clienteDto.Nome,
                Telefone = clienteDto.Telefone,
                Email = clienteDto.Email,
                Endereco = clienteDto.Endereco
            };

            _clienteRepository.Adicionar(cliente);
    }

    public async Task<ClienteDto> ObterPorId(int id)
    {
        Cliente cliente = await _clienteRepository.ObterPorId(id)
            ?? throw new ServiceException("Cliente não encontrado.");

        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Telefone = cliente.Telefone,
            Email = cliente.Email,
            Endereco = cliente.Endereco
        };
    }

    public void Atualizar(ClienteDto clienteDto)
    {
            Cliente cliente = this._clienteRepository.ObterPorId(clienteDto.Id).GetAwaiter().GetResult()
                  ?? throw new ServiceException("Cliente não encontrado.");

        cliente.Nome = clienteDto.Nome;
            cliente.Telefone = clienteDto.Telefone;
            cliente.Email = clienteDto.Email;
            cliente.Endereco = clienteDto.Endereco;

            this._clienteRepository.Atualizar(cliente);
    }
}
