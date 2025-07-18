using CRM.API.Middlewares;
using CRM.Infrastructure.DbContext;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;

namespace CRM.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    private string dadosConexao => Configuration.GetConnectionString("DefaultConnection");

    public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
    {
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
    }

    private void RegistrarContextos(IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddDbContext<CrmDbContext>(options =>
        {
            options.UseMySql(dadosConexao,
                ServerVersion.Create(new Version("8.0.28"), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql),
                m =>
                {
                    m.MigrationsAssembly("CRM.Infrastructure");
                    m.CommandTimeout(50000);
                    m.EnableRetryOnFailure(1, TimeSpan.FromSeconds(3), null);
                })
            .LogTo(s => System.Diagnostics.Debug.WriteLine(s))
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(true);
            options.UseLazyLoadingProxies();
        });
    }


    private void AdicionarSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "1.0",
                Title = "API do CRM"
            });
            s.CustomSchemaIds(x => x.FullName);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("pt-BR"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            using (var scope = app.ApplicationServices.CreateScope()) // Use app.ApplicationServices aqui
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<CrmDbContext>();
                    // Aplica todas as migrações pendentes ao banco de dados
                    dbContext.Database.Migrate();
                    // Para logs visíveis no console do Docker
                    Console.WriteLine("Migrações do banco de dados aplicadas com sucesso.");
                }
                catch (Exception ex)
                {
                    // Para logs visíveis no console do Docker
                    Console.Error.WriteLine($"Ocorreu um erro ao aplicar as migrações do banco de dados: {ex.Message}");
                    Console.Error.WriteLine(ex.StackTrace);
                }
            }
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
            c.RoutePrefix = "docs"; // Agora: https://localhost:7095/swagger
        });

        app.UseRouting();

        app.UseCors("AllowAll");

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
