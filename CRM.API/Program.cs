using CRM.API;

var builder = WebApplication.CreateBuilder(args);

Startup startup = new(builder.Configuration);
startup.ConfigureServices(builder.Services, builder.Environment);

var app = builder.Build();
startup.Configure(app, builder.Environment);

app.Run();
