﻿namespace CRM.Domain.Entidades;

public class Cliente : BaseModel<Cliente>
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }

    public bool CadastroCompleto()
    {
        return !string.IsNullOrWhiteSpace(Telefone) &&
           !string.IsNullOrWhiteSpace(Email) &&
           !string.IsNullOrWhiteSpace(Endereco);
    }
}
