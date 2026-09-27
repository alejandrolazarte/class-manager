namespace ClassManager.Security.Accounts;

public enum PasswordResetStatus
{
    Reset,
    InvalidToken,
    InvalidPassword,
}

public sealed record PasswordReset(PasswordResetStatus Status, string? ErrorDescription)
{
    public static PasswordReset Succeeded { get; } = new(PasswordResetStatus.Reset, null);

    public static PasswordReset InvalidToken { get; } = new(PasswordResetStatus.InvalidToken, null);

    public static PasswordReset InvalidPassword(string errorDescription) =>
        new(PasswordResetStatus.InvalidPassword, errorDescription);
}
