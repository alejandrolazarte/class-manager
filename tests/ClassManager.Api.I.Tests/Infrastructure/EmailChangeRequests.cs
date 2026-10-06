using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class EmailChangeRequests
{
    public const string ConfirmEmailChangeRoute = ApiRoutes.Authentication + ApiRoutes.ConfirmEmailChange;
    public const string WebAppConfirmEmailChangeUrl = "http://localhost:8081/confirm-email-change?token=";

    public static string UniqueNewEmail() => $"new-{Guid.NewGuid():N}@example.com";

    public static Task<HttpResponseMessage> PostEmailChangeAsync(this HttpClient httpClient, string newEmail, string currentPassword) =>
        httpClient.PostAsJsonAsync(ApiRoutes.MyEmailChange, new RequestEmailChangeCommand(newEmail, currentPassword), ApiRequests.JsonOptions);

    public static string EmailChangeTokenSentTo(this RecordingEmailTransport emailTransport, string email)
    {
        var body = emailTransport.SentTo(email)[^1].TextBody;
        var link = body.Split('\n').Single(line => line.StartsWith(WebAppConfirmEmailChangeUrl, StringComparison.Ordinal));
        return Uri.UnescapeDataString(link[WebAppConfirmEmailChangeUrl.Length..]);
    }

    public static Task<HttpResponseMessage> PostConfirmEmailChangeAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(ConfirmEmailChangeRoute, new ConfirmEmailChangeCommand(token), ApiRequests.JsonOptions);
}
