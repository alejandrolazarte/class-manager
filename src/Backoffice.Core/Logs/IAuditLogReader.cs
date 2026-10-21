namespace ClassManager.Backoffice.Core.Logs;

public interface IAuditLogReader
{
    Task<CommandLogPage> ListCommandsAsync(DateOnly day, IReadOnlyCollection<Guid>? tenantIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<SecurityLogEntry>> ListSecurityEventsAsync(DateOnly day, CancellationToken cancellationToken);
}
