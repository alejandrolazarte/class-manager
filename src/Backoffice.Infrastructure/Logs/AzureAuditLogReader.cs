using System.Globalization;
using System.Net;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using ClassManager.Backoffice.Core.Logs;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Backoffice.Infrastructure.Logs;

internal sealed class AzureAuditLogReader(TableServiceClient tableService, BlobContainerClient securityEvents) : IAuditLogReader
{
    public const int MaximumCommandsPerDay = 1000;

    private const string RowKeyUpperBoundSuffix = "_~";
    private const string PartitionSeparator = " or ";
    private const string ConditionSeparator = " and ";

    public bool IsConnected => true;

    public async Task<CommandLogPage> ListCommandsAsync(DateOnly day, IReadOnlyCollection<long>? tenantIds, CancellationToken cancellationToken)
    {
        var start = new DateTimeOffset(day.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var end = start.AddDays(1);
        var table = tableService.GetTableClient(TableCommandAuditStore.TableNameOf(start));
        var filter = string.Join(ConditionSeparator, ConditionsOf(start, end, tenantIds));
        var entries = new List<CommandLogEntry>();
        try
        {
            await foreach (var entity in table.QueryAsync<TableEntity>(filter, cancellationToken: cancellationToken))
            {
                if (entries.Count == MaximumCommandsPerDay)
                {
                    return new CommandLogPage(entries, IsTruncated: true);
                }

                entries.Add(EntryOf(entity));
            }
        }
        catch (RequestFailedException exception) when (exception.Status == (int)HttpStatusCode.NotFound)
        {
            return new CommandLogPage([], IsTruncated: false);
        }

        return new CommandLogPage(entries, IsTruncated: false);
    }

    public async Task<IReadOnlyList<SecurityLogEntry>> ListSecurityEventsAsync(DateOnly day, CancellationToken cancellationToken)
    {
        var blob = securityEvents.GetBlobClient(AppendBlobSecurityEventStore.BlobNameOf(new DateTimeOffset(day.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero)));
        try
        {
            var content = await blob.DownloadContentAsync(cancellationToken);
            return [.. AppendBlobSecurityEventStore.ReadLines(content.Value.Content.ToString()).Select(EntryOf)];
        }
        catch (RequestFailedException exception) when (exception.Status == (int)HttpStatusCode.NotFound)
        {
            return [];
        }
    }

    private static IEnumerable<string> ConditionsOf(DateTimeOffset start, DateTimeOffset end, IReadOnlyCollection<long>? tenantIds)
    {
        yield return TableClient.CreateQueryFilter($"RowKey gt {TableCommandAuditStore.RowKeyPrefixOf(end) + RowKeyUpperBoundSuffix}");
        yield return TableClient.CreateQueryFilter($"RowKey lt {TableCommandAuditStore.RowKeyPrefixOf(start) + RowKeyUpperBoundSuffix}");
        if (tenantIds is not null)
        {
            yield return $"({string.Join(PartitionSeparator, tenantIds.Select(tenantId => TableClient.CreateQueryFilter($"PartitionKey eq {tenantId.ToString(CultureInfo.InvariantCulture)}")))})";
        }
    }

    private static CommandLogEntry EntryOf(TableEntity entity) =>
        new(
            entity.GetDateTimeOffset(nameof(CommandAuditEntry.OccurredAt)) ?? DateTimeOffset.MinValue,
            long.TryParse(entity.PartitionKey, NumberStyles.None, CultureInfo.InvariantCulture, out var tenantId) ? tenantId : null,
            entity.GetString(nameof(CommandAuditEntry.Command)) ?? string.Empty,
            entity.GetGuid(nameof(CommandAuditEntry.AccountId)),
            entity.GetInt64(nameof(CommandAuditEntry.UserId)),
            entity.GetString(nameof(CommandAuditEntry.Outcome)) ?? string.Empty,
            entity.GetString(nameof(CommandAuditEntry.ErrorKind)),
            entity.GetString(nameof(CommandAuditEntry.ErrorCode)),
            entity.GetInt64(nameof(CommandAuditEntry.DurationMilliseconds)) ?? 0,
            entity.GetString(nameof(CommandAuditEntry.TraceId)),
            entity.GetString(nameof(CommandAuditEntry.Payload)),
            entity.GetBoolean(nameof(CommandAuditEntry.PayloadTruncated)) ?? false,
            entity.GetInt32(nameof(CommandAuditEntry.RowCount)),
            entity.GetString(nameof(CommandAuditEntry.Source)) ?? AuditSource.AppName);

    private static SecurityLogEntry EntryOf(SecurityEvent securityEvent) =>
        new(
            securityEvent.OccurredAt,
            securityEvent.Event.ToString(),
            securityEvent.TenantId,
            securityEvent.AccountId,
            securityEvent.UserId,
            securityEvent.Route,
            securityEvent.Method,
            securityEvent.StatusCode,
            securityEvent.IpAddress,
            securityEvent.UserAgent,
            securityEvent.Count,
            securityEvent.TraceId,
            securityEvent.Routes?.Select(route => new SecurityLogRoute(route.Method, route.Route, route.Count)).ToList(),
            securityEvent.OtherRouteRequests);
}
