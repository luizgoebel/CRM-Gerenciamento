namespace CRM.Domain.Entidades;

public class ValidationResult
{
    public List<string> Erros { get; } = [];

    public bool IsValid => !Erros.Any();

    public void AddError(string mensagem)
    {
        if (!string.IsNullOrWhiteSpace(mensagem))
            Erros.Add(mensagem);
    }
}
