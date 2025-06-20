namespace CRM.Domain.Entidades;

public class Cliente : BaseModel<Cliente>
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }

    public bool CadastroCompleto()
    {
        return ValidarCadastroCompleto().IsValid;
    }

    public void Alterar(string nome, string telefone, string? email, string? endereco)
    {
        this.Nome = nome;
        this.Telefone = telefone;
        this.Email = email ?? string.Empty;
        this.Endereco = endereco ?? string.Empty;

        ValidarCadastroParcial();
    }

    public ValidationResult ValidarCadastroParcial()
    {
        ValidationResult result = new();

        if (string.IsNullOrWhiteSpace(this.Nome))
            result.AddError("Nome do cliente é obrigatório.");
        if (string.IsNullOrWhiteSpace(this.Telefone))
            result.AddError("Telefone do cliente é obrigatório.");

        return result;
    }

    public ValidationResult ValidarCadastroCompleto()
    {
        var result = ValidarCadastroParcial();

        if (string.IsNullOrWhiteSpace(Email))
            result.AddError("E-mail do cliente é obrigatório.");

        if (string.IsNullOrWhiteSpace(Endereco))
            result.AddError("Endereço do cliente é obrigatório.");

        return result;
    }
}
