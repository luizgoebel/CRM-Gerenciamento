using CRM.API;

var builder = WebApplication.CreateBuilder(args);

Startup startup = new(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();
await startup.Configure(app, builder.Environment);

app.Run();
