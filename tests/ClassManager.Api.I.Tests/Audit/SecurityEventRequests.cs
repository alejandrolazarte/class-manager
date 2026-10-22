using ClassManager.Api.Audit;
using ClassManager.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed class SecurityEventRequests
{
    public const string ClientsRoute = "/api/clients";
    public const string IpAddress = "203.0.113.7";
    public const string UserAgent = "test-agent";

    public static readonly long TenantId = TestIds.Next();
    public static readonly long UserId = TestIds.Next();

    public RecordingSecurityEventService Events { get; } = new();

    public SecurityEventThrottle Throttle { get; } = new(Options.Create(new AuditLogOptions()));

    public SecurityEventBurstWorker BurstWorker => new(Throttle, Events, new FakeTimeProvider(CommandAuditEntries.OccurredAt));

    public async Task SendAsync(int statusCode, object? endpointMetadata = null, bool planLimit = false, string route = ClientsRoute, string userAgent = UserAgent)
    {
        var middleware = new SecurityEventMiddleware(
            httpContext =>
            {
                httpContext.Response.StatusCode = statusCode;
                if (planLimit)
                {
                    Subscriptions.AspNetCore.Endpoints.PlanLimitResponses.Mark(httpContext);
                }

                return Task.CompletedTask;
            },
            Events,
            Throttle,
            new FakeTimeProvider(CommandAuditEntries.OccurredAt));
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = HttpMethods.Post;
        httpContext.Request.Headers.UserAgent = userAgent;
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse(IpAddress);
        httpContext.SetEndpoint(new RouteEndpoint(
            _ => Task.CompletedTask,
            RoutePatternFactory.Parse(route),
            order: 0,
            endpointMetadata is null ? EndpointMetadataCollection.Empty : new EndpointMetadataCollection(endpointMetadata),
            displayName: route));

        await middleware.InvokeAsync(httpContext, new FixedCurrentUser(null, UserId), new FixedTenantContext(TenantId));
    }
}
