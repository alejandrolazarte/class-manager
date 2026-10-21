namespace ClassManager.Backoffice.Core.Logs;

public sealed record CommandLogEntry(
    DateTimeOffset OccurredAt,
    Guid? TenantId,
    string Command,
    Guid? AccountId,
    Guid? UserId,
    string Outcome,
    string? ErrorKind,
    string? ErrorCode,
    long DurationMilliseconds,
    string? TraceId,
    string? Payload,
    bool PayloadTruncated,
    int? RowCount,
    string Source);
