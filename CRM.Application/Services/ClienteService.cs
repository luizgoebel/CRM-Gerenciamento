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

    public async Task<int> Adicionar(ClienteDto clienteDto)
    {
        try
        {
            var cliente = new Cliente
            {
                Nome = clienteDto.Nome,
                Telefone = clienteDto.Telefone,
                Email = clienteDto.Email,
                Endereco = clienteDto.Endereco
            };

            return await _clienteRepository.Adicionar(cliente);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao adicionar cliente.", ex);
        }
    }

    public async Task<ClienteDto> ObterPorId(int id)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorId(id)
                ?? throw new ServiceException($"Cliente com ID {id} não encontrado.");

            return new ClienteDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Telefone = cliente.Telefone,
                Email = cliente.Email,
                Endereco = cliente.Endereco
            };
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao obter cliente por ID.", ex);
        }
    }

    public async Task Atualizar(int id, ClienteDto clienteDto)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorId(id)
                ?? throw new ServiceException($"Cliente com ID {id} não encontrado.");

            cliente.Nome = clienteDto.Nome;
            cliente.Telefone = clienteDto.Telefone;
            cliente.Email = clienteDto.Email;
            cliente.Endereco = clienteDto.Endereco;

            await _clienteRepository.Atualizar(cliente);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Erro ao atualizar cliente.", ex);
        }
    }
}
