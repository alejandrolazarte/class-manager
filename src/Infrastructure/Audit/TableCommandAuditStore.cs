using System.Collections.Concurrent;
using System.Globalization;
using Azure.Data.Tables;

namespace ClassManager.Infrastructure.Audit;

public sealed class TableCommandAuditStore(TableServiceClient serviceClient) : IAuditLogStore<CommandAuditEntry>
{
    public const string NoBusinessPartition = "none";

    private const string TableNamePrefix = "Commands";
    private const string TableMonthFormat = "yyyyMM";
    private const string InvertedTicksFormat = "D19";
    private const string RowKeySeparator = "_";
    private const string RowKeyGuidFormat = "N";
    private const int MaximumTransactionSize = 100;

    private readonly ConcurrentDictionary<string, TableClient> _createdTables = new();

    public async Task WriteAsync(IReadOnlyList<CommandAuditEntry> entries, CancellationToken cancellationToken)
    {
        var groups = entries.GroupBy(entry => (Table: TableNameOf(entry), Partition: PartitionOf(entry)));
        foreach (var group in groups)
        {
            var table = await GetCreatedTableAsync(group.Key.Table, cancellationToken);
            foreach (var transaction in group.Chunk(MaximumTransactionSize))
            {
                var actions = transaction.Select(entry => new TableTransactionAction(TableTransactionActionType.Add, RowOf(entry)));
                await table.SubmitTransactionAsync(actions, cancellationToken);
            }
        }
    }

    private async Task<TableClient> GetCreatedTableAsync(string tableName, CancellationToken cancellationToken)
    {
        if (_createdTables.TryGetValue(tableName, out var created))
        {
            return created;
        }

        var table = serviceClient.GetTableClient(tableName);
        await table.CreateIfNotExistsAsync(cancellationToken);
        _createdTables[tableName] = table;
        return table;
    }

    private static string TableNameOf(CommandAuditEntry entry) =>
        TableNamePrefix + entry.OccurredAt.UtcDateTime.ToString(TableMonthFormat, CultureInfo.InvariantCulture);

    private static string PartitionOf(CommandAuditEntry entry) =>
        entry.TenantId?.ToString() ?? NoBusinessPartition;

    private static TableEntity RowOf(CommandAuditEntry entry)
    {
        var invertedTicks = DateTimeOffset.MaxValue.UtcTicks - entry.OccurredAt.UtcTicks;
        var rowKey = invertedTicks.ToString(InvertedTicksFormat, CultureInfo.InvariantCulture)
            + RowKeySeparator
            + Guid.NewGuid().ToString(RowKeyGuidFormat, CultureInfo.InvariantCulture);
        return new TableEntity(PartitionOf(entry), rowKey)
        {
            [nameof(CommandAuditEntry.OccurredAt)] = entry.OccurredAt,
            [nameof(CommandAuditEntry.Command)] = entry.Command,
            [nameof(CommandAuditEntry.AccountId)] = entry.AccountId,
            [nameof(CommandAuditEntry.UserId)] = entry.UserId,
            [nameof(CommandAuditEntry.Outcome)] = entry.Outcome.ToString(),
            [nameof(CommandAuditEntry.ErrorKind)] = entry.ErrorKind,
            [nameof(CommandAuditEntry.ErrorCode)] = entry.ErrorCode,
            [nameof(CommandAuditEntry.DurationMilliseconds)] = entry.DurationMilliseconds,
            [nameof(CommandAuditEntry.TraceId)] = entry.TraceId,
            [nameof(CommandAuditEntry.Payload)] = entry.Payload,
            [nameof(CommandAuditEntry.PayloadTruncated)] = entry.PayloadTruncated,
            [nameof(CommandAuditEntry.RowCount)] = entry.RowCount,
        };
    }
}
