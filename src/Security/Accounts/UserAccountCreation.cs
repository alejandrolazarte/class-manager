namespace ClassManager.Security.Accounts;

public enum UserAccountCreationStatus
{
    Created,
    EmailTaken,
    InvalidPassword,
    Invalid,
}

public sealed record UserAccountCreation(UserAccountCreationStatus Status, Guid UserId, string? ErrorDescription)
{
    public static UserAccountCreation EmailTaken { get; } = new(UserAccountCreationStatus.EmailTaken, Guid.Empty, null);

    public static UserAccountCreation Created(Guid userId) => new(UserAccountCreationStatus.Created, userId, null);

    public static UserAccountCreation InvalidPassword(string errorDescription) =>
        new(UserAccountCreationStatus.InvalidPassword, Guid.Empty, errorDescription);

    public static UserAccountCreation Invalid(string errorDescription) =>
        new(UserAccountCreationStatus.Invalid, Guid.Empty, errorDescription);
}
