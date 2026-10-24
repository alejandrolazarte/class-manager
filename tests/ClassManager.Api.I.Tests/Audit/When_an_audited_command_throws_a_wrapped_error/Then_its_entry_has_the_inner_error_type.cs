using ClassManager.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Audit.When_an_audited_command_throws_a_wrapped_error;

public sealed class Then_its_entry_has_the_inner_error_type
{
    [Fact]
    public async Task Then_its_entry_has_the_inner_error_type_Run()
    {
        var wrappedError = new DbUpdateException("Saving failed.", new InvalidOperationException("Not a SQL Server error."));
        var auditLog = new RecordingCommandAuditService();
        var behavior = CommandAuditBehaviors.Create(auditLog);

        await Should.ThrowAsync<DbUpdateException>(() => behavior.HandleAsync(
            CommandAuditBehaviors.Command,
            () => Task.FromException<Result<string>>(wrappedError),
            CancellationToken.None));

        var entry = auditLog.Entries.ShouldHaveSingleItem();
        (entry.ErrorKind, entry.ErrorCode).ShouldBe((nameof(DbUpdateException), nameof(InvalidOperationException)));
    }
}
