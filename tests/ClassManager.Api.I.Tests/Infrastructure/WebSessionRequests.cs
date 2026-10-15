using ClassManager.Api.Authentication;
using ClassManager.Core.UseCases.Authentication;
using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class WebSessionRequests
{
    public static async Task<HttpResponseMessage> SignUpAndSignInFromWebAsync(this HttpClient httpClient, RecordingEmailTransport emailTransport)
    {
        var command = AuthenticationRequests.SignUpCommand();
        await httpClient.SignUpAsync(emailTransport, command);
        return await httpClient.PostFromWebAsync(
            AuthenticationRequests.SignInRoute,
            new SignInCommand(command.Email, AuthenticationRequests.OwnerPassword));
    }

    public static async Task<HttpResponseMessage> PostFromWebAsync(
        this HttpClient httpClient,
        string route,
        object body,
        string? refreshTokenCookieValue = null,
        bool sendsCookieHeader = true)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(route, UriKind.Relative))
        {
            Content = JsonContent.Create(body, body.GetType(), options: ApiRequests.JsonOptions),
        };
        if (sendsCookieHeader)
        {
            request.Headers.Add(RefreshTokenCookie.RequestHeaderName, RefreshTokenCookie.RequestHeaderValue);
        }
        if (refreshTokenCookieValue is not null)
        {
            request.Headers.Add(HeaderNames.Cookie, $"{RefreshTokenCookie.Name}={refreshTokenCookieValue}");
        }
        return await httpClient.SendAsync(request);
    }

    public static SetCookieHeaderValue RefreshTokenCookieOf(this HttpResponseMessage response) =>
        SetCookieHeaderValue.ParseList(response.Headers.GetValues(HeaderNames.SetCookie).ToList())
            .Single(cookie => cookie.Name == RefreshTokenCookie.Name);
}
