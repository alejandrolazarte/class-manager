namespace ClassManager.Security.Accounts;

public interface IPasswordResetService
{
    /// <summary>
    /// Creates a single-use password reset token for the account with this email.
    /// Returns <see langword="null"/> when no account uses the email.
    /// </summary>
    Task<string?> CreateTokenAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Sets a new password with a token from <see cref="CreateTokenAsync"/>. On success every reset token of the user
    /// is used up, the lockout is cleared and every session of the user is revoked.
    /// </summary>
    Task<PasswordReset> ResetAsync(string token, string newPassword, CancellationToken cancellationToken);
}
