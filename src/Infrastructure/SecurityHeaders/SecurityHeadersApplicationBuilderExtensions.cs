using Microsoft.AspNetCore.Builder;

namespace ClassManager.Infrastructure.SecurityHeaders;

public static class SecurityHeadersApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app) =>
        app.UseMiddleware<SecurityHeadersMiddleware>();
}
