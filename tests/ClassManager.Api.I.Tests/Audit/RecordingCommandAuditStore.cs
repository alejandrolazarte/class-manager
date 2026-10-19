using System.Collections.Concurrent;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed class RecordingCommandAuditStore(int failingWrites = 0) : ICommandAuditStore
{
    private int _remainingFailures = failingWrites;

    public ConcurrentQueue<CommandAuditEntry> Written { get; } = new();

    public int Writes { get; private set; }

    public Task WriteAsync(IReadOnlyList<CommandAuditEntry> entries, CancellationToken cancellationToken)
    {
        Writes++;
        if (_remainingFailures > 0)
        {
            _remainingFailures--;
            throw new InvalidOperationException("Table Storage is not available.");
        }

        foreach (var entry in entries)
        {
            Written.Enqueue(entry);
        }

        return Task.CompletedTask;
    }
}
