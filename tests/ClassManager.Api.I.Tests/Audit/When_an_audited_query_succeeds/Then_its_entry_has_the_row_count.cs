using ClassManager.Core.Common;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_an_audited_query_succeeds;

public sealed class Then_its_entry_has_the_row_count
{
    private const int ExportedRows = 342;

    [Fact]
    public async Task Then_its_entry_has_the_row_count_Run()
    {
        var auditLog = new RecordingCommandAuditService();
        var behavior = CommandAuditBehaviors.CreateForQuery<SampleAuditedQuery>(auditLog);

        await behavior.HandleAsync(new SampleAuditedQuery("students"), () => Task.FromResult(Result.Success(new SampleRows(ExportedRows))), CancellationToken.None);

        var entry = auditLog.Entries.ShouldHaveSingleItem();
        (entry.Command, entry.TenantId, entry.UserId, entry.Outcome, entry.Payload, entry.RowCount).ShouldBe((
            nameof(SampleAuditedQuery),
            CommandAuditBehaviors.TenantId,
            CommandAuditBehaviors.UserId,
            CommandOutcome.Succeeded,
            """{"module":"students"}""",
            ExportedRows));
    }
}
