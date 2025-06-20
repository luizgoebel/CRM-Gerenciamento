namespace CRM.Domain.Entidades;

public abstract class BaseModel<T> : IBaseModel where T : BaseModel<T>
{
    public DateTime? DataCriacao { get; set; }
    public DateTime DataModificacao { get; set; }
}

public interface IBaseModel
{
    DateTime? DataCriacao { get; set; }
    DateTime DataModificacao { get; set; }
}

