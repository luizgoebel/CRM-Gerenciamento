using CRM.API.Middlewares;
using CRM.Infrastructure.DbContext;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System;
using System.Threading;
using System.Threading.Tasks;
using CRM.Application;
using CRM.Infrastructure;
using Microsoft.Extensions.Logging; // Adicionado para ILogger

namespace CRM.API;

public class Startup
{
    private readonly ILogger<Startup> _logger; // Adicionado logger

    public Startup(IConfiguration configuration, ILogger<Startup> logger) // Injetar ILogger
    {
        Configuration = configuration;
        _logger = logger; // Atribuir logger
    }

    public IConfiguration Configuration { get; }
    private string dadosConexao => Configuration.GetConnectionString("DefaultConnection");

    public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
    {
        _logger.LogInformation("ConfigureServices: Iniciando configuração dos serviços."); // Log
        services.AddMemoryCache();

        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
            .AddNewtonsoftJson();

        RegistrarContextos(services, env);
        Dependencias.RecuperarServicos(services);
        Dependencias.RecuperarRepositorios(services);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
        });

        AdicionarSwagger(services);
        _logger.LogInformation("ConfigureServices: Configuração dos serviços concluída."); // Log
    }

    private void RegistrarContextos(IServiceCollection services, IWebHostEnvironment env)
    {
        _logger.LogInformation("RegistrarContextos: Registrando DbContext."); // Log
        services.AddDbContext<CrmDbContext>(options =>
        {
            options.UseMySql(dadosConexao,
                ServerVersion.Create(new Version("8.0.28"), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql),
                m =>
                {
                    m.MigrationsAssembly("CRM.Infrastructure");
                    m.CommandTimeout(50000);
                    m.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                })
            .LogTo(s => System.Diagnostics.Debug.WriteLine(s))
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(true);
            options.UseLazyLoadingProxies();
        });
        _logger.LogInformation("RegistrarContextos: DbContext registrado."); // Log
    }

    private void AdicionarSwagger(IServiceCollection services)
    {
        _logger.LogInformation("AdicionarSwagger: Adicionando SwaggerGen."); // Log
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "1.0",
                Title = "API do CRM"
            });
            s.CustomSchemaIds(x => x.FullName);
        });
        _logger.LogInformation("AdicionarSwagger: SwaggerGen adicionado."); // Log
    }

    // O método Configure agora é 'async Task' para permitir 'await Task.Delay' e 'await MigrateAsync'
    public async Task Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        _logger.LogInformation("Configure: Iniciando configuração da aplicação."); // Log

        // ----- INÍCIO: Lógica para aplicar migrações (reintroduzida com retry) -----
        const int maxRetries = 5;
        const int delayMilliseconds = 5000; // 5 segundos

        for (int i = 0; i < maxRetries; i++)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    _logger.LogInformation($"Tentativa {i + 1}/{maxRetries}: Tentando aplicar migrações do banco de dados."); // Log
                    var dbContext = services.GetRequiredService<CrmDbContext>();
                    await dbContext.Database.MigrateAsync(); // 'await' é crucial aqui
                    _logger.LogInformation("Migrações do banco de dados aplicadas com sucesso."); // Log
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Tentativa {i + 1}/{maxRetries}: Ocorreu um erro ao aplicar as migrações do banco de dados."); // Log de erro
                    if (i < maxRetries - 1)
                    {
                        _logger.LogInformation($"Aguardando {delayMilliseconds / 1000} segundos antes de tentar novamente..."); 
                        await Task.Delay(delayMilliseconds); // CORRIGIDO: Usar Task.Delay para não bloquear a thread
                    }
                    else
                    {
                        _logger.LogError("Todas as tentativas de aplicar migrações falharam. A aplicação pode não funcionar corretamente."); // Log de erro final
                    }
                }
            }
        }
        // ----- FIM: Lógica para aplicar migrações -----

        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API do CRM v1");
            c.RoutePrefix = "docs";
        });

        app.UseRouting();

        app.UseCors("AllowAll");

        app.UseMiddleware<ExceptionMiddleware>();

        // NOVO: Adiciona um endpoint de health check simples
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/health", async context =>
            {
                await context.Response.WriteAsync("OK");
                _logger.LogInformation("Health check endpoint acessado."); // Log
            });
        });

        _logger.LogInformation("Configure: Configuração da aplicação concluída. Aplicação pronta para receber requisições."); // Log
    }
}
