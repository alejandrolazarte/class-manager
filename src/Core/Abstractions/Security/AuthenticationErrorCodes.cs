namespace ClassManager.Core.Abstractions.Security;

public static class AuthenticationErrorCodes
{
    public const string EmailTaken = "auth.email_taken";
    public const string InvalidCredentials = "auth.invalid_credentials";
    public const string LockedOut = "auth.locked_out";
    public const string InvalidRefreshToken = "auth.invalid_refresh_token";
    public const string InvalidPasswordResetToken = "auth.invalid_password_reset_token";

    public const string EmailTakenMessage = "An account with this email already exists.";
    public const string InvalidRefreshTokenMessage = "The session has expired. Sign in again.";
    public const string InvalidPasswordResetTokenMessage = "The link is invalid, expired or already used. Ask for a new one.";
}
