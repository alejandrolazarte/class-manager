using ClassManager.Backoffice.Api;
using ClassManager.Backoffice.Infrastructure;
using ClassManager.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBackofficeInfrastructure();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

var app = builder.Build();

app.MapHealthChecks(BackofficeApiRoutes.Health);

app.Run();
