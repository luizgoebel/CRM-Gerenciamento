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
            await HandleExceptionAsync(httpContext, ex, ServiceExceptionStatusCode, ex.ObjetoErro);
        }
        catch (DomainException ex)
        {
            await HandleExceptionAsync(httpContext, ex, DomainExceptionStatusCode, ex.ObjetoErro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado do sistema.");
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
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new DetalhesDoErro
            {
                StatusCode = statusCode,
                Mensagem = exception.Message,
                ObjetoErro = objetoErro
            }, _jsonOptions));
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}