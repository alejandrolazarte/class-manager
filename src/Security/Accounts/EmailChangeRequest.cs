namespace ClassManager.Security.Accounts;

public enum EmailChangeRequestStatus
{
    Requested,
    InvalidPassword,
    EmailTaken,
    SameEmail,
    UnknownUser,
}

public sealed record EmailChangeRequest(EmailChangeRequestStatus Status, string? Token, string? CurrentEmail)
{
    public static EmailChangeRequest InvalidPassword { get; } = new(EmailChangeRequestStatus.InvalidPassword, null, null);

    public static EmailChangeRequest EmailTaken { get; } = new(EmailChangeRequestStatus.EmailTaken, null, null);

    public static EmailChangeRequest SameEmail { get; } = new(EmailChangeRequestStatus.SameEmail, null, null);

    public static EmailChangeRequest UnknownUser { get; } = new(EmailChangeRequestStatus.UnknownUser, null, null);

    public static EmailChangeRequest Requested(string token, string currentEmail) =>
        new(EmailChangeRequestStatus.Requested, token, currentEmail);
}
