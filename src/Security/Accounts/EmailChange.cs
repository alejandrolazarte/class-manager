namespace ClassManager.Security.Accounts;

public enum EmailChangeStatus
{
    Changed,
    InvalidToken,
    EmailTaken,
}

public sealed record EmailChange(EmailChangeStatus Status, Guid UserId, string? PreviousEmail, string? NewEmail)
{
    public static EmailChange InvalidToken { get; } = new(EmailChangeStatus.InvalidToken, Guid.Empty, null, null);

    public static EmailChange EmailTaken { get; } = new(EmailChangeStatus.EmailTaken, Guid.Empty, null, null);

    public static EmailChange Changed(Guid userId, string previousEmail, string newEmail) =>
        new(EmailChangeStatus.Changed, userId, previousEmail, newEmail);
}
