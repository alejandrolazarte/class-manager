using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.Audit;

internal sealed partial class AuditLogWorker<TEntry>(
    AuditLogChannel<TEntry> channel,
    IAuditLogStore<TEntry> store,
    ILogger<AuditLogWorker<TEntry>> logger)
    : BackgroundService
{
    private const int BatchSize = 100;

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        while (channel.Reader.TryPeek(out _))
        {
            await WriteBatchAsync(cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await channel.Reader.WaitToReadAsync(stoppingToken))
            {
                await WriteBatchAsync(CancellationToken.None);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task WriteBatchAsync(CancellationToken cancellationToken)
    {
        var batch = new List<TEntry>(BatchSize);
        while (batch.Count < BatchSize && channel.Reader.TryRead(out var entry))
        {
            batch.Add(entry);
        }

        try
        {
            await store.WriteAsync(batch, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogWriteFailed(logger, exception, batch.Count, typeof(TEntry).Name);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Writing {EntryCount} {EntryType} audit entries failed; they are dropped")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception, int entryCount, string entryType);
}
