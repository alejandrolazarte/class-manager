using ClassManager.Core.Abstractions.Email;
using Microsoft.Extensions.Configuration;

namespace ClassManager.Infrastructure.Email;

internal sealed class WebAppLinks(IConfiguration configuration) : IWebAppLinks
{
    public const string WebAppUrlSetting = "WebApp:Url";
    public const string ResetPasswordPath = "/reset-password?token=";
    public const string ConfirmEmailChangePath = "/confirm-email-change?token=";
    public const string AcceptInvitationPath = "/accept-invitation?token=";
    public const string AcceptStudentAppInvitationPath = "/accept-student-invitation?token=";
    public const string AuthorizeStudentAppPath = "/authorize-student-app?token=";
    public const string StudentAppOrdersPath = "/student-app/orders";
    public const string TeamOrdersPath = "/today/orders";

    private const string MissingWebAppUrlMessage = "Set " + WebAppUrlSetting + " to the web app address.";

    public string ResetPassword(string token) =>
        WebAppUrl() + ResetPasswordPath + Uri.EscapeDataString(token);

    public string ConfirmEmailChange(string token) =>
        WebAppUrl() + ConfirmEmailChangePath + Uri.EscapeDataString(token);

    public string AcceptInvitation(string token) =>
        WebAppUrl() + AcceptInvitationPath + Uri.EscapeDataString(token);

    public string AcceptStudentAppInvitation(string token) =>
        WebAppUrl() + AcceptStudentAppInvitationPath + Uri.EscapeDataString(token);

    public string AuthorizeStudentApp(string token) =>
        WebAppUrl() + AuthorizeStudentAppPath + Uri.EscapeDataString(token);

    public string StudentAppOrders() => WebAppUrl() + StudentAppOrdersPath;

    public string TeamOrders() => WebAppUrl() + TeamOrdersPath;

    private string WebAppUrl() =>
        (configuration[WebAppUrlSetting] is { Length: > 0 } webAppUrl
            ? webAppUrl
            : throw new InvalidOperationException(MissingWebAppUrlMessage)).TrimEnd('/');
}
