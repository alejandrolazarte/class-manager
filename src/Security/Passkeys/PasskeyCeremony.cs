namespace ClassManager.Security.Passkeys;

public sealed class PasskeyCeremony
{
    public const int CeremonyIdHashLength = 64;
    public const int KindMaxLength = 20;

    public Guid Id { get; init; }
    public string CeremonyIdHash { get; init; } = string.Empty;
    public PasskeyCeremonyKind Kind { get; init; }
    public Guid? UserId { get; init; }
    public string? State { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
}
