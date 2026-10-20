using System.Text.Json.Serialization;
using ClassManager.Backoffice.Api;
using ClassManager.Backoffice.Api.Authentication;
using ClassManager.Backoffice.Api.Endpoints;
using ClassManager.Backoffice.Infrastructure;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.SecurityHeaders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddBackofficeInfrastructure();
builder.Services.AddBackofficeAuthentication(builder.Configuration);
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

app.Run();
