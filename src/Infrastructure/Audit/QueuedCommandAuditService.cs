namespace ClassManager.Infrastructure.Audit;

public sealed class QueuedCommandAuditService(AuditLogChannel<CommandAuditEntry> channel) : ICommandAuditService
{
    public void Record(CommandAuditEntry entry) => channel.TryWrite(entry);
}
