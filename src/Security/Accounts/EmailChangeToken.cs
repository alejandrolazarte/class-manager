namespace ClassManager.Security.Accounts;

public sealed class EmailChangeToken
{
    public const int TokenHashLength = 64;
    public const int NewEmailMaxLength = 256;

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string NewEmail { get; init; } = string.Empty;
    public string TokenHash { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset? UsedAt { get; init; }
}
