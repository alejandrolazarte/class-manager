using System.Diagnostics;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.Audit;

internal sealed class SecurityEventMiddleware(
    RequestDelegate next,
    ISecurityEventService securityEvents,
    SecurityEventThrottle throttle,
    TimeProvider timeProvider)
{
    public const string UnknownAddress = "unknown";
    public const int UserAgentMaximumLength = 256;

    public async Task InvokeAsync(HttpContext httpContext, ICurrentUser currentUser, ITenantContext tenantContext)
    {
        var endpoint = httpContext.GetEndpoint();
        await next(httpContext);

        var statusCode = httpContext.Response.StatusCode;
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? UnknownAddress;
        var routeTemplate = (endpoint as RouteEndpoint)?.RoutePattern.RawText;
        var method = httpContext.Request.Method;
        if (EventOf(httpContext, endpoint, statusCode) is not { } securityEvent)
        {
            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                throttle.CountUnauthorized(ipAddress, RouteOf(method, routeTemplate), UserAgentOf(httpContext));
            }

            return;
        }

        if (!throttle.TryRecord(ipAddress))
        {
            throttle.CountSuppressed(ipAddress, RouteOf(method, routeTemplate), UserAgentOf(httpContext));
            return;
        }

        securityEvents.Record(new SecurityEvent(
            timeProvider.GetUtcNow(),
            securityEvent,
            TenantIdOf(tenantContext),
            currentUser.AccountId,
            currentUser.UserId,
            routeTemplate,
            method,
            statusCode,
            ipAddress,
            UserAgentOf(httpContext),
            Count: null,
            Activity.Current?.TraceId.ToString()));
    }

    private static RequestRoute? RouteOf(string method, string? routeTemplate) =>
        routeTemplate is null ? null : new RequestRoute(method, routeTemplate);

    private static string UserAgentOf(HttpContext httpContext)
    {
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        return userAgent.Length > UserAgentMaximumLength ? userAgent[..UserAgentMaximumLength] : userAgent;
    }

    private static SecurityEventType? EventOf(HttpContext httpContext, Endpoint? endpoint, int statusCode)
    {
        if (statusCode == StatusCodes.Status403Forbidden)
        {
            return PlanLimitResponses.IsMarked(httpContext) ? SecurityEventType.PlanLimit : SecurityEventType.Forbidden;
        }

        var isRejection = statusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status423Locked;
        return isRejection ? endpoint?.Metadata.GetMetadata<SecurityEventOnRejection>()?.Event : null;
    }

    private static Guid? TenantIdOf(ITenantContext tenantContext)
    {
        try
        {
            return tenantContext.TenantId;
        }
        catch (TenantNotResolvedException)
        {
            return null;
        }
    }
}
