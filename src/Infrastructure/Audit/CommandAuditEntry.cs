namespace ClassManager.Infrastructure.Audit;

public sealed record CommandAuditEntry(
    DateTimeOffset OccurredAt,
    Guid? TenantId,
    string Command,
    Guid? AccountId,
    Guid? UserId,
    CommandOutcome Outcome,
    string? ErrorKind,
    string? ErrorCode,
    long DurationMilliseconds,
    string? TraceId,
    string? Payload,
    bool PayloadTruncated);
