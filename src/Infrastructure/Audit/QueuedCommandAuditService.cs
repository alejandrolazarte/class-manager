namespace ClassManager.Infrastructure.Audit;

public sealed class QueuedCommandAuditService(CommandAuditChannel channel) : ICommandAuditService
{
    public void Record(CommandAuditEntry entry) => channel.TryWrite(entry);
}
