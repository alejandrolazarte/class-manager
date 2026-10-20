using ClassManager.Core.Common;

namespace ClassManager.Api.I.Tests.Audit.When_a_query_is_not_audited;

public sealed class Then_no_entry_is_recorded
{
    [Fact]
    public async Task Then_no_entry_is_recorded_Run()
    {
        var auditLog = new RecordingCommandAuditService();
        var behavior = CommandAuditBehaviors.CreateForQuery<SampleQuery>(auditLog);

        await behavior.HandleAsync(new SampleQuery("students"), () => Task.FromResult(Result.Success(new SampleRows(1))), CancellationToken.None);

        auditLog.Entries.ShouldBeEmpty();
    }
}
