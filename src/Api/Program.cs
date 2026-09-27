using System.Text.Json.Serialization;

using ClassManager.Api;
using ClassManager.Api.Authentication;
using ClassManager.Api.Cors;
using ClassManager.Api.Endpoints;
using ClassManager.Api.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddAppCors(builder.Configuration);
builder.Services.AddAppAuthentication();
builder.Services.AddInfrastructure();
builder.Services.AddUseCases();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors(AppCorsServiceCollectionExtensions.AppPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapHealthChecks(ApiRoutes.Health).AllowAnonymous();
app.MapAuthenticationEndpoints();
app.MapBusinessEndpoints();
app.MapClientEndpoints();
app.MapStudentEndpoints();
app.MapInstructorEndpoints();
app.MapClassGroupEndpoints();
app.MapEnrollmentEndpoints();
app.MapSessionEndpoints();
app.MapPrivateLessonEndpoints();
app.MapFeeEndpoints();
app.MapClassPackEndpoints();
app.MapImportExportEndpoints();

app.Run();
