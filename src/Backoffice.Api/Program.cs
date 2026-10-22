using System.Text.Json.Serialization;
using ClassManager.Backoffice.Api;
using ClassManager.Backoffice.Api.Authentication;
using ClassManager.Backoffice.Api.Endpoints;
using ClassManager.Backoffice.Infrastructure;
using ClassManager.Infrastructure.Audit;
using ClassManager.Infrastructure.Json;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.SecurityHeaders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.WriteIdsAsStrings().Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddBackofficeInfrastructure();
builder.Services.AddBackofficeAuthentication(builder.Configuration);
builder.Services.AddHostedService<AuditLogWorker<CommandAuditEntry>>();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseSecurityHeaders();
app.UseCors(BackofficeAuthenticationServiceCollectionExtensions.CorsPolicyName);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks(BackofficeApiRoutes.Health).AllowAnonymous();
app.MapBackofficeAuthenticationEndpoints();
app.MapCurrentOperatorEndpoints();
app.MapBrandEndpoints();
app.MapOperatorEndpoints();
app.MapLogEndpoints();

app.Run();
