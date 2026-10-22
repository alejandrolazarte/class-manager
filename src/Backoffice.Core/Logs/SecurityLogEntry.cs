namespace ClassManager.Backoffice.Core.Logs;

public sealed record SecurityLogEntry(
    DateTimeOffset OccurredAt,
    string Event,
    Guid? TenantId,
    Guid? AccountId,
    Guid? UserId,
    string? Route,
    string? Method,
    int? StatusCode,
    string? IpAddress,
    string? UserAgent,
    int? Count,
    string? TraceId,
    IReadOnlyList<SecurityLogRoute>? Routes = null,
    int? OtherRouteRequests = null);
