using ClassManager.Core.Abstractions.Email;
using Microsoft.Extensions.Configuration;

namespace ClassManager.Infrastructure.Email;

internal sealed class WebAppLinks(IConfiguration configuration) : IWebAppLinks
{
    public const string WebAppUrlSetting = "WebApp:Url";
    public const string ResetPasswordPath = "/reset-password?token=";

    private const string MissingWebAppUrlMessage = "Set " + WebAppUrlSetting + " to the web app address.";

    public string ResetPassword(string token) =>
        WebAppUrl() + ResetPasswordPath + Uri.EscapeDataString(token);

    private string WebAppUrl() =>
        (configuration[WebAppUrlSetting] is { Length: > 0 } webAppUrl
            ? webAppUrl
            : throw new InvalidOperationException(MissingWebAppUrlMessage)).TrimEnd('/');
}
