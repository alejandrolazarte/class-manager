namespace ClassManager.Core.Abstractions.Security;

public static class AuthenticationErrorCodes
{
    public const string EmailTaken = "auth.email_taken";
    public const string InvalidCredentials = "auth.invalid_credentials";
    public const string LockedOut = "auth.locked_out";
    public const string InvalidRefreshToken = "auth.invalid_refresh_token";

    public const string EmailTakenMessage = "An account with this email already exists.";
    public const string InvalidRefreshTokenMessage = "The session has expired. Sign in again.";
}
