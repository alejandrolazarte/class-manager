namespace ClassManager.Security.Tokens;

public sealed class RefreshToken
{
    public const int TokenHashLength = 64;

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid? TenantId { get; init; }
    public string TokenHash { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset? RevokedAt { get; init; }
    public Guid? ReplacedByTokenId { get; init; }
}
