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
        await next(httpContext);

        var statusCode = httpContext.Response.StatusCode;
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? UnknownAddress;
        if (EventOf(httpContext, statusCode) is not { } securityEvent)
        {
            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                throttle.CountUnauthorized(ipAddress);
            }

            return;
        }

        if (!throttle.TryRecord(ipAddress))
        {
            throttle.CountSuppressed(ipAddress);
            return;
        }

        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        securityEvents.Record(new SecurityEvent(
            timeProvider.GetUtcNow(),
            securityEvent,
            TenantIdOf(tenantContext),
            currentUser.AccountId,
            currentUser.UserId,
            (httpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText,
            httpContext.Request.Method,
            statusCode,
            ipAddress,
            userAgent.Length > UserAgentMaximumLength ? userAgent[..UserAgentMaximumLength] : userAgent,
            Count: null,
            Activity.Current?.TraceId.ToString()));
    }

    private static SecurityEventType? EventOf(HttpContext httpContext, int statusCode)
    {
        if (statusCode == StatusCodes.Status403Forbidden)
        {
            return PlanLimitResponses.IsMarked(httpContext) ? null : SecurityEventType.Forbidden;
        }

        var isRejection = statusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status423Locked;
        return isRejection ? httpContext.GetEndpoint()?.Metadata.GetMetadata<SecurityEventOnRejection>()?.Event : null;
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
