using System.Text.Json.Serialization;

using ClassManager.Api;
using ClassManager.Api.Authentication;
using ClassManager.Api.Cors;
using ClassManager.Api.Endpoints;
using ClassManager.Api.ErrorHandling;
using ClassManager.Api.Notifications;
using ClassManager.Api.Orders;
using ClassManager.Api.SecurityHeaders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddAppCors(builder.Configuration);
builder.Services.AddAppAuthentication();
builder.Services.AddInfrastructure();
builder.Services.AddUseCases();
builder.Services.AddSingleton<IExpiredOrderCancellationService, ExpiredOrderCancellationService>();
builder.Services.AddHostedService<ExpiredOrderCancellationWorker>();
builder.Services.AddHostedService<PushWorker>();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
}

var app = builder.Build();

app.UseForwardedHeaders();
app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().RequireAuthorization(AuthorizationPolicies.AnyAccount);
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors(AppCorsServiceCollectionExtensions.AppPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.UseActiveSubscriptionForChanges();
app.UseRateLimiter();

app.MapHealthChecks(ApiRoutes.Health).AllowAnonymous();
app.MapAuthenticationEndpoints();
app.MapBusinessEndpoints();
app.MapBrandEndpoints();
app.MapMemberEndpoints();
app.MapRoleEndpoints();
app.MapClientEndpoints();
app.MapFamilyEndpoints();
app.MapTeamNotificationEndpoints();
app.MapAnnouncementEndpoints();
app.MapStudentEndpoints();
app.MapInstructorEndpoints();
app.MapClassGroupEndpoints();
app.MapEnrollmentEndpoints();
app.MapSessionEndpoints();
app.MapPrivateLessonEndpoints();
app.MapFeeEndpoints();
app.MapClassPackEndpoints();
app.MapProductEndpoints();
app.MapOrderEndpoints();
app.MapImportExportEndpoints();

app.Run();
