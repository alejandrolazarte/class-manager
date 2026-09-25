namespace ClassManager.Core.Abstractions.Security;

public enum CredentialVerificationStatus
{
    Verified,
    InvalidCredentials,
    LockedOut,
}

public sealed record CredentialVerification(CredentialVerificationStatus Status, Guid UserId, string Email)
{
    public static CredentialVerification InvalidCredentials { get; } =
        new(CredentialVerificationStatus.InvalidCredentials, Guid.Empty, string.Empty);

    public static CredentialVerification LockedOut { get; } =
        new(CredentialVerificationStatus.LockedOut, Guid.Empty, string.Empty);

    public static CredentialVerification Verified(Guid userId, string email) =>
        new(CredentialVerificationStatus.Verified, userId, email);
}
