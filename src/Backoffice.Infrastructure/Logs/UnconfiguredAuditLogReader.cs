using ClassManager.Backoffice.Core.Logs;

namespace ClassManager.Backoffice.Infrastructure.Logs;

internal sealed class UnconfiguredAuditLogReader : IAuditLogReader
{
    public bool IsConnected => false;

    public Task<CommandLogPage> ListCommandsAsync(DateOnly day, IReadOnlyCollection<Guid>? tenantIds, CancellationToken cancellationToken) =>
        Task.FromResult(new CommandLogPage([], IsTruncated: false));

    public Task<IReadOnlyList<SecurityLogEntry>> ListSecurityEventsAsync(DateOnly day, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<SecurityLogEntry>>([]);
}
