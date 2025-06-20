namespace CRM.Domain.Entidades;

public class Produto : BaseModel<Produto>
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }

    public ValidationResult Validar()
    {
        ValidationResult result = new();

        if (string.IsNullOrWhiteSpace(this.Nome))
            result.AddError("Nome do produto é obrigatório.");
        if (this.Preco <= 0)
            result.AddError("Definir o preço do produto.");

        return result;
    }

    public void Alterar(string nome, decimal preco)
    {
        this.Nome = nome;
        this.Preco = preco;
    }
}
