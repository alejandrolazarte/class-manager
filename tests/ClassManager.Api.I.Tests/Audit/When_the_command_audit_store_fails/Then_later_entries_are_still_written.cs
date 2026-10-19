using ClassManager.Api.Audit;
using ClassManager.Infrastructure.Audit;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClassManager.Api.I.Tests.Audit.When_the_command_audit_store_fails;

public sealed class Then_later_entries_are_still_written
{
    private static readonly TimeSpan WaitLimit = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task Then_later_entries_are_still_written_Run()
    {
        var channel = new CommandAuditChannel();
        var store = new RecordingCommandAuditStore(failingWrites: 1);
        var auditLog = new QueuedCommandAuditService(channel);
        using var worker = new CommandAuditWorker(channel, store, NullLogger<CommandAuditWorker>.Instance);
        await worker.StartAsync(CancellationToken.None);
        auditLog.Record(CommandAuditEntries.Succeeded(Guid.CreateVersion7()));
        var waitUntil = DateTime.UtcNow + WaitLimit;
        while (store.Writes == 0 && DateTime.UtcNow < waitUntil)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(20));
        }

        var later = CommandAuditEntries.Succeeded(Guid.CreateVersion7());
        auditLog.Record(later);
        await worker.StopAsync(CancellationToken.None);

        store.Written.ShouldBe([later]);
    }
}
