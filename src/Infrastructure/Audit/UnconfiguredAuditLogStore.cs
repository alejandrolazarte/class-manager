namespace ClassManager.Infrastructure.Audit;

internal sealed class UnconfiguredAuditLogStore<TEntry> : IAuditLogStore<TEntry>
{
    public Task WriteAsync(IReadOnlyList<TEntry> entries, CancellationToken cancellationToken) => Task.CompletedTask;
}
