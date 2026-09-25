namespace ClassManager.Security.Accounts;

public enum CredentialCheckStatus
{
    Verified,
    InvalidCredentials,
    LockedOut,
}

public sealed record CredentialCheck(CredentialCheckStatus Status, Guid UserId, string Email)
{
    public static CredentialCheck InvalidCredentials { get; } = new(CredentialCheckStatus.InvalidCredentials, Guid.Empty, string.Empty);

    public static CredentialCheck LockedOut { get; } = new(CredentialCheckStatus.LockedOut, Guid.Empty, string.Empty);

    public static CredentialCheck Verified(Guid userId, string email) => new(CredentialCheckStatus.Verified, userId, email);
}
