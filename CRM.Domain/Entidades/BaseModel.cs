namespace CRM.Domain.Entidades;

public abstract class BaseModel<T> where T : BaseModel<T>
{
    public DateTime? DataCriacao { get; set; }
    public DateTime DataModificacao { get; set; }

    public BaseModel()
    {
        if (DataCriacao is null)
            DataCriacao = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

        DataModificacao = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
    }
}
