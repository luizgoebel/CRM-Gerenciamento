namespace CRM.Application.Exceptions;

public class DomainException : Exception
{
    public object ObjetoErro { get; }
    public DomainException(string message) : base(message)
    {
    }
    public DomainException(string message, object objetoErro = null) : base(message)
    {
        ObjetoErro = objetoErro;
    }
}
