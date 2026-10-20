using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Net.Http.Headers;

namespace ClassManager.Backoffice.Api.I.Tests.Infrastructure;

public static class BackofficeRequests
{
    public const string RefreshTokenCookieName = "laztty-backoffice-refresh-token";
    public const string AccessRequestCodeParameter = "accessRequestCode=";

    private const string SignInRoute = "/api/auth/signin/google";
    private const string RefreshRoute = "/api/auth/refresh";
    private const string CurrentOperatorRoute = "/api/me";
    private const string CookieRequestHeader = "X-Refresh-Token-Cookie";
    private const string CookieRequestHeaderValue = "true";
    private const string BearerScheme = "Bearer";

    public static async Task<HttpResponseMessage> SignInWithGoogleAsync(this HttpClient client, string subject, string email)
    {
        using var challenge = await client.GetAsync(new Uri(
            $"{SignInRoute}?{FakeGoogleHandler.SubjectParameter}={Uri.EscapeDataString(subject)}&{FakeGoogleHandler.EmailParameter}={Uri.EscapeDataString(email)}",
            UriKind.Relative));
        challenge.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        return await client.GetAsync(challenge.Headers.Location);
    }

    public static async Task<HttpResponseMessage> PostRefreshAsync(this HttpClient client)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(RefreshRoute, UriKind.Relative));
        request.Headers.Add(CookieRequestHeader, CookieRequestHeaderValue);
        return await client.SendAsync(request);
    }

    public static async Task<string> AccessTokenOfAsync(HttpResponseMessage refreshResponse) =>
        (await refreshResponse.Content.ReadFromJsonAsync<AccessTokenResponse>())!.AccessToken;

    public static Task<HttpResponseMessage> GetCurrentOperatorAsync(this HttpClient client, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(CurrentOperatorRoute, UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue(BearerScheme, accessToken);
        return client.SendAsync(request);
    }

    public static SetCookieHeaderValue? RefreshTokenCookieOf(HttpResponseMessage response) =>
        response.Headers.TryGetValues(HeaderNames.SetCookie, out var cookies)
            ? SetCookieHeaderValue.ParseList(cookies.ToList()).SingleOrDefault(cookie => cookie.Name == RefreshTokenCookieName)
            : null;

    private sealed record AccessTokenResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt);
}
