using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed class RecordingCommandAuditService : ICommandAuditService
{
    public List<CommandAuditEntry> Entries { get; } = [];

    public void Record(CommandAuditEntry entry) => Entries.Add(entry);
}
