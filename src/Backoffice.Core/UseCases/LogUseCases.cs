using ClassManager.Backoffice.Core.Logs;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;

namespace ClassManager.Backoffice.Core.UseCases;

[AuditedQuery]
public sealed record ListCommandLogQuery(DateOnly? Day, Guid? BrandId) : IQuery;

[AuditedQuery]
public sealed record ListSecurityEventsQuery(DateOnly? Day) : IQuery;

public sealed record CommandLogItemResponse(
    DateTimeOffset OccurredAt,
    string Command,
    string Outcome,
    string? ErrorKind,
    string? ErrorCode,
    long DurationMilliseconds,
    string? TraceId,
    string? Payload,
    bool PayloadTruncated,
    int? RowCount,
    string Source,
    Guid? BrandId,
    string? BrandName,
    string? BranchName,
    Guid? UserId,
    string? UserName);

public sealed record CommandLogResponse(DateOnly Day, IReadOnlyList<CommandLogItemResponse> Entries, bool IsTruncated, bool IsConnected = true) : ICountedRows
{
    public int RowCount => Entries.Count;
}

public sealed record SecurityEventResponse(
    DateTimeOffset OccurredAt,
    string Event,
    string? Route,
    string? Method,
    int? StatusCode,
    string? IpAddress,
    string? UserAgent,
    int? Count,
    string? TraceId,
    Guid? BrandId,
    string? BrandName,
    string? BranchName,
    string? UserName,
    IReadOnlyList<SecurityLogRoute>? Routes,
    int? OtherRouteRequests);

public sealed record SecurityEventListResponse(DateOnly Day, IReadOnlyList<SecurityEventResponse> Events, bool IsConnected = true) : ICountedRows
{
    public int RowCount => Events.Count;
}

public sealed class ListCommandLogUseCase(IAuditLogReader reader, IAuditLogDirectory directory, TimeProvider timeProvider)
    : IUseCase<ListCommandLogQuery, CommandLogResponse>
{
    public async Task<Result<CommandLogResponse>> ExecuteAsync(ListCommandLogQuery command, CancellationToken cancellationToken)
    {
        var day = command.Day ?? DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (!reader.IsConnected)
        {
            return new CommandLogResponse(day, [], IsTruncated: false, IsConnected: false);
        }

        IReadOnlyCollection<Guid>? tenantIds = null;
        if (command.BrandId is { } brandId)
        {
            tenantIds = await directory.ListBranchIdsAsync(brandId, cancellationToken);
            if (tenantIds.Count == 0)
            {
                return new CommandLogResponse(day, [], IsTruncated: false);
            }
        }

        var page = await reader.ListCommandsAsync(day, tenantIds, cancellationToken);
        var branches = await directory.FindBranchesAsync(AuditLogLookups.DistinctIds(page.Entries.Select(entry => entry.TenantId)), cancellationToken);
        var userNames = await directory.FindUserNamesAsync(AuditLogLookups.DistinctIds(page.Entries.Select(entry => entry.UserId)), cancellationToken);
        return new CommandLogResponse(
            day,
            [.. page.Entries
                .OrderByDescending(entry => entry.OccurredAt)
                .Select(entry =>
                {
                    var branch = AuditLogLookups.BranchOf(branches, entry.TenantId);
                    return new CommandLogItemResponse(
                        entry.OccurredAt,
                        entry.Command,
                        entry.Outcome,
                        entry.ErrorKind,
                        entry.ErrorCode,
                        entry.DurationMilliseconds,
                        entry.TraceId,
                        entry.Payload,
                        entry.PayloadTruncated,
                        entry.RowCount,
                        entry.Source,
                        branch?.BrandId,
                        branch?.BrandName,
                        branch?.BranchName,
                        entry.UserId,
                        AuditLogLookups.NameOf(userNames, entry.UserId));
                })],
            page.IsTruncated);
    }
}

public sealed class ListSecurityEventsUseCase(IAuditLogReader reader, IAuditLogDirectory directory, TimeProvider timeProvider)
    : IUseCase<ListSecurityEventsQuery, SecurityEventListResponse>
{
    public async Task<Result<SecurityEventListResponse>> ExecuteAsync(ListSecurityEventsQuery command, CancellationToken cancellationToken)
    {
        var day = command.Day ?? DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (!reader.IsConnected)
        {
            return new SecurityEventListResponse(day, [], IsConnected: false);
        }

        var events = await reader.ListSecurityEventsAsync(day, cancellationToken);
        var branches = await directory.FindBranchesAsync(AuditLogLookups.DistinctIds(events.Select(entry => entry.TenantId)), cancellationToken);
        var userNames = await directory.FindUserNamesAsync(AuditLogLookups.DistinctIds(events.Select(entry => entry.UserId)), cancellationToken);
        return new SecurityEventListResponse(
            day,
            [.. events
                .OrderByDescending(entry => entry.OccurredAt)
                .Select(entry =>
                {
                    var branch = AuditLogLookups.BranchOf(branches, entry.TenantId);
                    return new SecurityEventResponse(
                        entry.OccurredAt,
                        entry.Event,
                        entry.Route,
                        entry.Method,
                        entry.StatusCode,
                        entry.IpAddress,
                        entry.UserAgent,
                        entry.Count,
                        entry.TraceId,
                        branch?.BrandId,
                        branch?.BrandName,
                        branch?.BranchName,
                        AuditLogLookups.NameOf(userNames, entry.UserId),
                        entry.Routes,
                        entry.OtherRouteRequests);
                })]);
    }
}
