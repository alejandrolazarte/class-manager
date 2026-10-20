namespace ClassManager.Backoffice.Api.Authentication;

internal static class BackofficeAuthenticationSchemes
{
    public const string External = "backoffice-external";
    public const string Google = "google";
    public const string GoogleCallbackPath = "/signin-google";
    public const string ExternalCookieName = "laztty-backoffice-external";
    public const string SubjectClaim = "sub";
    public const string EmailClaim = "email";
}
