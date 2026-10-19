using ClassManager.Core.Common;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_an_audited_command_succeeds;

public sealed class Then_its_entry_masks_personal_fields
{
    [Fact]
    public async Task Then_its_entry_masks_personal_fields_Run()
    {
        var auditLog = new RecordingCommandAuditService();
        var behavior = CommandAuditBehaviors.Create(auditLog);

        await behavior.HandleAsync(CommandAuditBehaviors.Command, () => Task.FromResult(Result.Success("done")), CancellationToken.None);

        var entry = auditLog.Entries.ShouldHaveSingleItem();
        (entry.Command, entry.TenantId, entry.AccountId, entry.UserId, entry.Outcome, entry.Payload).ShouldBe((
            nameof(SampleCommand),
            CommandAuditBehaviors.TenantId,
            CommandAuditBehaviors.AccountId,
            CommandAuditBehaviors.UserId,
            CommandOutcome.Succeeded,
            $$"""{"clientId":"{{CommandAuditBehaviors.Command.ClientId}}","fullName":"[personal]","amount":25000}"""));
    }
}
