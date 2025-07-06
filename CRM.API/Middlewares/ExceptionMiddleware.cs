using CRM.Application.Exceptions;
using CRM.Domain.Entidades;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRM.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private const int ExceptionStatusCode = 500;
    private const int ServiceExceptionStatusCode = 503;
    private const int DomainExceptionStatusCode = 400;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ServiceException ex)
        {
            // Logar a ServiceException com seus detalhes
            _logger.LogError(ex, "ServiceException capturada: {ErrorMessage}", ex.Message);
            await HandleExceptionAsync(httpContext, ex, ServiceExceptionStatusCode, ex.ObjetoErro);
        }
        catch (DomainException ex)
        {
            // Logar a DomainException com seus detalhes
            _logger.LogError(ex, "DomainException capturada: {ErrorMessage}", ex.Message);
            await HandleExceptionAsync(httpContext, ex, DomainExceptionStatusCode, ex.ObjetoErro);
        }
        catch (Exception ex) // <--- Esta é a seção que precisa da alteração
        {
            // Logar a exceção completa, incluindo a InnerException, se existir
            _logger.LogError(ex, "Erro inesperado do sistema. Detalhes completos da exceção: {ErrorMessage}", ex.Message);

            // Adicione este bloco para logar a InnerException especificamente
            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, "Inner Exception: {InnerErrorMessage}", ex.InnerException.Message);
            }

            // Você pode adicionar mais detalhes aqui, como a stack trace, etc.
            // _logger.LogError(ex, "Stack Trace: {StackTrace}", ex.StackTrace);


            await HandleExceptionAsync(httpContext, ex, ExceptionStatusCode);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        object objetoErro = null)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        // Adicione um log aqui também para ver o que está sendo retornado ao cliente
        _logger.LogWarning("Retornando erro {StatusCode} para o cliente: {Mensagem}", statusCode, exception.Message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(new DetalhesDoErro() // Use JsonSerializer.Serialize aqui
        {
            StatusCode = context.Response.StatusCode,
            Mensagem = exception.Message,
            ObjetoErro = objetoErro
        }, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve, // Opcional: para lidar com referências cíclicas se ObjetoErro for complexo
            WriteIndented = true // Opcional: para JSON formatado nos logs
        }));
    }
}