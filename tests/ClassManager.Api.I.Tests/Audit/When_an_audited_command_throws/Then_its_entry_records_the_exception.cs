using ClassManager.Core.Common;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_an_audited_command_throws;

public sealed class Then_its_entry_records_the_exception
{
    [Fact]
    public async Task Then_its_entry_records_the_exception_Run()
    {
        var auditLog = new RecordingCommandAuditService();
        var behavior = CommandAuditBehaviors.Create(auditLog);

        await Should.ThrowAsync<TimeoutException>(() => behavior.HandleAsync(
            CommandAuditBehaviors.Command,
            () => Task.FromException<Result<string>>(new TimeoutException()),
            CancellationToken.None));

        var entry = auditLog.Entries.ShouldHaveSingleItem();
        (entry.Outcome, entry.ErrorKind).ShouldBe((CommandOutcome.Exception, nameof(TimeoutException)));
    }
}
