using ClassManager.Core.Abstractions.Email;
using Microsoft.Extensions.Configuration;

namespace ClassManager.Infrastructure.Email;

internal sealed class WebAppLinks(IConfiguration configuration) : IWebAppLinks
{
    public const string WebAppUrlSetting = "WebApp:Url";
    public const string ResetPasswordPath = "/reset-password?token=";
    public const string AcceptInvitationPath = "/accept-invitation?token=";
    public const string AcceptFamilyInvitationPath = "/accept-family-invitation?token=";
    public const string FamilyOrdersPath = "/family/orders";
    public const string TeamOrdersPath = "/today/orders";

    private const string MissingWebAppUrlMessage = "Set " + WebAppUrlSetting + " to the web app address.";

    public string ResetPassword(string token) =>
        WebAppUrl() + ResetPasswordPath + Uri.EscapeDataString(token);

    public string AcceptInvitation(string token) =>
        WebAppUrl() + AcceptInvitationPath + Uri.EscapeDataString(token);

    public string AcceptFamilyInvitation(string token) =>
        WebAppUrl() + AcceptFamilyInvitationPath + Uri.EscapeDataString(token);

    public string FamilyOrders() => WebAppUrl() + FamilyOrdersPath;

    public string TeamOrders() => WebAppUrl() + TeamOrdersPath;

    private string WebAppUrl() =>
        (configuration[WebAppUrlSetting] is { Length: > 0 } webAppUrl
            ? webAppUrl
            : throw new InvalidOperationException(MissingWebAppUrlMessage)).TrimEnd('/');
}
