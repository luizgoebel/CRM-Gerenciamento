namespace CRM.Domain.Entidades;

public class Produto : BaseModel<Produto>
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal? Preco { get; set; }
}
