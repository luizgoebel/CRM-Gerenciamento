﻿namespace CRM.Application.DTOs;

public class ClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
}
