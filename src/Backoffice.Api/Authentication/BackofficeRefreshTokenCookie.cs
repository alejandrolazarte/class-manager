using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Backoffice.Api.Authentication;

internal static class BackofficeRefreshTokenCookie
{
    public const string RequestHeaderName = "X-Refresh-Token-Cookie";
    public const string RequestHeaderValue = "true";
    public const string Name = "laztty-backoffice-refresh-token";

    public static bool IsRequested(HttpRequest request) =>
        request.Headers[RequestHeaderName] == RequestHeaderValue;

    public static string? Of(HttpRequest request) => request.Cookies[Name];

    public static void Keep(HttpResponse response, TokenResponse tokens) =>
        response.Cookies.Append(Name, tokens.RefreshToken, OptionsUntil(tokens.RefreshTokenExpiresAt));

    public static void Forget(HttpResponse response) =>
        response.Cookies.Delete(Name, OptionsUntil(expiresAt: null));

    private static CookieOptions OptionsUntil(DateTimeOffset? expiresAt) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = BackofficeApiRoutes.Authentication,
        Expires = expiresAt,
        IsEssential = true,
    };
}
