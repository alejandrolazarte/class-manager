namespace ClassManager.Infrastructure.Audit;

public interface ICommandAuditStore
{
    Task WriteAsync(IReadOnlyList<CommandAuditEntry> entries, CancellationToken cancellationToken);
}
