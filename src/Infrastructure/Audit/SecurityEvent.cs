namespace ClassManager.Infrastructure.Audit;

public sealed record SecurityEvent(
    DateTimeOffset OccurredAt,
    SecurityEventType Event,
    long? TenantId,
    Guid? AccountId,
    long? UserId,
    string? Route,
    string? Method,
    int? StatusCode,
    string? IpAddress,
    string? UserAgent,
    int? Count,
    string? TraceId,
    IReadOnlyList<SecurityEventRoute>? Routes = null,
    int? OtherRouteRequests = null);
