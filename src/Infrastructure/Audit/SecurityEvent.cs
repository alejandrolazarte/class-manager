namespace ClassManager.Infrastructure.Audit;

public sealed record SecurityEvent(
    DateTimeOffset OccurredAt,
    SecurityEventType Event,
    Guid? TenantId,
    Guid? AccountId,
    Guid? UserId,
    string? Route,
    string? Method,
    int? StatusCode,
    string? IpAddress,
    string? UserAgent,
    int? Count,
    string? TraceId);
