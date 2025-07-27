using CRM.API;
using Microsoft.Extensions.Logging; // Necess�rio para ILogger

var builder = WebApplication.CreateBuilder(args);

// Adiciona ILogger<Startup> ao cont�iner de servi�os para que possa ser injetado
builder.Services.AddSingleton<ILogger<Startup>>(provider =>
    provider.GetRequiredService<ILoggerFactory>().CreateLogger<Startup>());

// Cria uma inst�ncia de Startup, passando a configura��o e o logger
Startup startup = new(builder.Configuration, builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>());

startup.ConfigureServices(builder.Services, builder.Environment);

var app = builder.Build();
await startup.Configure(app, builder.Environment);

app.Run();
