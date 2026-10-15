using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.Authentication;

public static class RefreshTokenCookie
{
    public const string RequestHeaderName = "X-Refresh-Token-Cookie";
    public const string RequestHeaderValue = "true";
    public const string Name = "class-manager-refresh-token";
    public const string Path = ApiRoutes.Authentication;

    public static string? RefreshTokenOf(HttpRequest request, string? bodyRefreshToken) =>
        string.IsNullOrWhiteSpace(bodyRefreshToken) && IsRequested(request)
            ? request.Cookies[Name]
            : bodyRefreshToken;

    public static object Keep(HttpContext httpContext, TokenResponse tokens)
    {
        if (!IsRequested(httpContext.Request))
        {
            return tokens;
        }

        httpContext.Response.Cookies.Append(Name, tokens.RefreshToken, OptionsUntil(tokens.RefreshTokenExpiresAt));
        return CookieTokenResponse.From(tokens);
    }

    public static void Forget(HttpContext httpContext)
    {
        if (IsRequested(httpContext.Request))
        {
            httpContext.Response.Cookies.Delete(Name, OptionsUntil(expiresAt: null));
        }
    }

    private static bool IsRequested(HttpRequest request) =>
        request.Headers[RequestHeaderName] == RequestHeaderValue;

    private static CookieOptions OptionsUntil(DateTimeOffset? expiresAt) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = Path,
        Expires = expiresAt,
        IsEssential = true,
    };
}
