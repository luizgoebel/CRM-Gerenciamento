using CRM.API.Middlewares;
using CRM.Infrastructure.DbContext;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
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
    }

    private void RegistrarContextos(IServiceCollection services, IWebHostEnvironment env)
    {
        services
           .AddDbContext<CrmDbContext>(options =>
           {
               options.UseMySql(dadosConexao,
                   ServerVersion.Create(
                       new Version("8.0.28"),
                       Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql),
                   m =>
                   {
                       m.MigrationsAssembly("Diario.Infrastructure.Migrations");
                       m.CommandTimeout(50000);
                   }
               )
               .LogTo(s => System.Diagnostics.Debug.WriteLine(s))
               .EnableDetailedErrors(true)
               .EnableSensitiveDataLogging(true);
               options.UseLazyLoadingProxies();
           });
    }
    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        CultureInfo[] supportedCultures = new[] { new CultureInfo("pt-BR") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Versão única");
            //    c.RoutePrefix = string.Empty;
            //    c.OAuthClientId("Diario-Api-Swagger");
            //    c.OAuthClientSecret("diarioapiswagger-tec1429@");
            //    c.OAuthAppName("Diario-Api-Swagger");
            //    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            //});
        }

        //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        app.Use((context, next) =>
        {
            context.Request.Scheme = "https";
            return next();
        });

        app.UseRouting();
        //app.UseAuthentication();
        //app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
