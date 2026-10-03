using Microsoft.Extensions.Primitives;

namespace ClassManager.Api.SecurityHeaders;

public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public const string ContentTypeOptionsHeader = "X-Content-Type-Options";
    public const string ContentTypeOptionsValue = "nosniff";
    public const string FrameOptionsHeader = "X-Frame-Options";
    public const string FrameOptionsValue = "DENY";
    public const string ReferrerPolicyHeader = "Referrer-Policy";
    public const string ReferrerPolicyValue = "no-referrer";

    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(static state =>
        {
            var headers = ((HttpContext)state).Response.Headers;
            headers[ContentTypeOptionsHeader] = new StringValues(ContentTypeOptionsValue);
            headers[FrameOptionsHeader] = new StringValues(FrameOptionsValue);
            headers[ReferrerPolicyHeader] = new StringValues(ReferrerPolicyValue);
            return Task.CompletedTask;
        }, context);

        return next(context);
    }
}
