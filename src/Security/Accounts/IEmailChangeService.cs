namespace ClassManager.Security.Accounts;

public interface IEmailChangeService
{
    /// <summary>
    /// Checks the current password and creates a single-use token that moves the account to <paramref name="newEmail"/>.
    /// The caller sends the token to the new address so only its owner can confirm the change.
    /// </summary>
    Task<EmailChangeRequest> RequestAsync(Guid userId, string currentPassword, string newEmail, CancellationToken cancellationToken);

    /// <summary>
    /// Moves the account to the email of a token from <see cref="RequestAsync"/>. On success every email change token
    /// of the user is used up and the email becomes the user name used to sign in.
    /// </summary>
    Task<EmailChange> ConfirmAsync(string token, CancellationToken cancellationToken);
}
