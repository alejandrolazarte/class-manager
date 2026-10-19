namespace ClassManager.Infrastructure.Audit;

public interface IAuditLogStore<in TEntry>
{
    Task WriteAsync(IReadOnlyList<TEntry> entries, CancellationToken cancellationToken);
}
