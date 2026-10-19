namespace ClassManager.Infrastructure.Audit;

internal sealed class UnconfiguredCommandAuditStore : ICommandAuditStore
{
    public Task WriteAsync(IReadOnlyList<CommandAuditEntry> entries, CancellationToken cancellationToken) => Task.CompletedTask;
}
