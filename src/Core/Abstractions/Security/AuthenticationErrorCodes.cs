namespace ClassManager.Core.Abstractions.Security;

public static class AuthenticationErrorCodes
{
    public const string EmailTaken = "auth.email_taken";
    public const string InvalidCredentials = "auth.invalid_credentials";
    public const string LockedOut = "auth.locked_out";
    public const string InvalidRefreshToken = "auth.invalid_refresh_token";
    public const string InvalidPasswordResetToken = "auth.invalid_password_reset_token";
    public const string InvalidCurrentPassword = "auth.invalid_current_password";
    public const string SameEmail = "auth.same_email";
    public const string InvalidEmailChangeToken = "auth.invalid_email_change_token";
    public const string TooYoungForOwnAccount = "auth.too_young_for_own_account";
    public const string OwnerMustBeAdult = "auth.owner_must_be_adult";

    public const string EmailTakenMessage = "An account with this email already exists.";
    public const string InvalidRefreshTokenMessage = "The session has expired. Sign in again.";
    public const string InvalidPasswordResetTokenMessage = "The link is invalid, expired or already used. Ask for a new one.";
    public const string InvalidCurrentPasswordMessage = "The current password is not correct.";
    public const string SameEmailMessage = "This is already the email of the account.";
    public const string TooYoungForOwnAccountMessage = "This person is too young for an account of their own; the client who pays sees their classes.";
    public const string OwnerMustBeAdultMessage = "The owner of a business must be an adult.";
    public const string InvalidEmailChangeTokenMessage = "The link is invalid, expired or already used. Ask for the change again.";
}
