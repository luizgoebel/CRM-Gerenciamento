using CRM.API;
using Microsoft.Extensions.Logging; // Necessário para ILogger

var builder = WebApplication.CreateBuilder(args);

// Adiciona ILogger<Startup> ao contêiner de serviços para que possa ser injetado
builder.Services.AddSingleton<ILogger<Startup>>(provider =>
    provider.GetRequiredService<ILoggerFactory>().CreateLogger<Startup>());

// Cria uma instância de Startup, passando a configuração e o logger
Startup startup = new(builder.Configuration, builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>());

startup.ConfigureServices(builder.Services, builder.Environment);

var app = builder.Build();
startup.Configure(app, builder.Environment);

app.Run();
