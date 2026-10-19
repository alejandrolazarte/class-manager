using ClassManager.Api.Audit;
using ClassManager.Infrastructure.Audit;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClassManager.Api.I.Tests.Audit.When_command_audit_entries_are_queued;

public sealed class Then_the_worker_writes_them_before_stopping
{
    private const int EntryCount = 250;

    [Fact]
    public async Task Then_the_worker_writes_them_before_stopping_Run()
    {
        var channel = new CommandAuditChannel();
        var store = new RecordingCommandAuditStore();
        var auditLog = new QueuedCommandAuditService(channel);
        using var worker = new CommandAuditWorker(channel, store, NullLogger<CommandAuditWorker>.Instance);
        await worker.StartAsync(CancellationToken.None);

        for (var entryNumber = 0; entryNumber < EntryCount; entryNumber++)
        {
            auditLog.Record(CommandAuditEntries.Succeeded(Guid.CreateVersion7()));
        }

        await worker.StopAsync(CancellationToken.None);

        store.Written.Count.ShouldBe(EntryCount);
    }
}
