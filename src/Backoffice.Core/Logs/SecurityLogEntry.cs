namespace ClassManager.Backoffice.Core.Logs;

public sealed record SecurityLogEntry(
    DateTimeOffset OccurredAt,
    string Event,
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
    IReadOnlyList<SecurityLogRoute>? Routes = null,
    int? OtherRouteRequests = null);
