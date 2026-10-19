using ClassManager.Core.Common;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_an_audited_command_fails;

public sealed class Then_its_entry_has_the_error
{
    private const string ErrorCode = "client_not_found";

    [Fact]
    public async Task Then_its_entry_has_the_error_Run()
    {
        var auditLog = new RecordingCommandAuditService();
        var behavior = CommandAuditBehaviors.Create(auditLog);

        await behavior.HandleAsync(CommandAuditBehaviors.Command, () => Task.FromResult(Result.NotFound<string>("Client not found.", ErrorCode)), CancellationToken.None);

        var entry = auditLog.Entries.ShouldHaveSingleItem();
        (entry.Outcome, entry.ErrorKind, entry.ErrorCode).ShouldBe((CommandOutcome.Failed, nameof(ErrorKind.NotFound), ErrorCode));
    }
}
