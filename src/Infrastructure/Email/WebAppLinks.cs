using ClassManager.Core.Abstractions.Email;
using Microsoft.Extensions.Configuration;

namespace ClassManager.Infrastructure.Email;

internal sealed class WebAppLinks(IConfiguration configuration) : IWebAppLinks
{
    public const string WebAppUrlSetting = "WebApp:Url";
    public const string SignInPath = "/sign-in";
    public const string CompleteSignUpPath = "/complete-sign-up?token=";
    public const string ResetPasswordPath = "/reset-password?token=";
    public const string EmailParameter = "&email=";
    public const string ConfirmEmailChangePath = "/confirm-email-change?token=";
    public const string AcceptInvitationPath = "/accept-invitation?token=";
    public const string AcceptStudentAppInvitationPath = "/accept-student-invitation?token=";
    public const string AuthorizeStudentAppPath = "/authorize-student-app?token=";
    public const string StudentAppOrdersPath = "/student-app/orders";
    public const string TeamOrdersPath = "/today/orders";
    public const string PlanSettingsPath = "/settings/plan";
    public const string CheckoutParameter = "?checkout=";
    public const string CheckoutSucceeded = "success";
    public const string CheckoutCancelled = "cancelled";
    public const string WebFontsPath = "/fonts/";

    private const string MissingWebAppUrlMessage = "Set " + WebAppUrlSetting + " to the web app address.";

    public string SignIn() => WebAppUrl() + SignInPath;

    public string CompleteSignUp(string token, string email) =>
        WebAppUrl() + CompleteSignUpPath + Uri.EscapeDataString(token) + EmailParameter + Uri.EscapeDataString(email);

    public string ResetPassword(string token, string email) =>
        WebAppUrl() + ResetPasswordPath + Uri.EscapeDataString(token) + EmailParameter + Uri.EscapeDataString(email);

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

    public string PlanSettings() => WebAppUrl() + PlanSettingsPath;

    public string PlanCheckoutSucceeded() => PlanSettings() + CheckoutParameter + CheckoutSucceeded;

    public string PlanCheckoutCancelled() => PlanSettings() + CheckoutParameter + CheckoutCancelled;

    public Uri WebFonts() => new(WebAppUrl() + WebFontsPath);

    private string WebAppUrl() =>
        (configuration[WebAppUrlSetting] is { Length: > 0 } webAppUrl
            ? webAppUrl
            : throw new InvalidOperationException(MissingWebAppUrlMessage)).TrimEnd('/');
}
