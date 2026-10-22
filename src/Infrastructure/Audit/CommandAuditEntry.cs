namespace ClassManager.Infrastructure.Audit;

public sealed record CommandAuditEntry(
    DateTimeOffset OccurredAt,
    long? TenantId,
    string Command,
    Guid? AccountId,
    long? UserId,
    CommandOutcome Outcome,
    string? ErrorKind,
    string? ErrorCode,
    long DurationMilliseconds,
    string? TraceId,
    string? Payload,
    bool PayloadTruncated,
    int? RowCount = null,
    string Source = AuditSource.AppName);
