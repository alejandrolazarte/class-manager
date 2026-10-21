using ClassManager.Infrastructure.Audit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.Audit;

internal static class CommandAuditBehaviors
{
    public static readonly Guid TenantId = Guid.CreateVersion7();
    public static readonly Guid AccountId = Guid.CreateVersion7();
    public static readonly Guid UserId = Guid.CreateVersion7();

    public static readonly SampleCommand Command = new(Guid.CreateVersion7(), "Ana María Pérez", 25000m);

    public static CommandAuditBehavior<SampleCommand, string> Create(RecordingCommandAuditService auditLog) => new(Recorder(auditLog));

    public static QueryAuditBehavior<TQuery, SampleRows> CreateForQuery<TQuery>(RecordingCommandAuditService auditLog)
        where TQuery : Core.UseCases.IQuery => new(Recorder(auditLog));

    private static UseCaseAuditRecorder Recorder(RecordingCommandAuditService auditLog) => new(
        auditLog,
        new FixedTenantContext(TenantId),
        new FixedCurrentUser(AccountId, UserId),
        new FakeTimeProvider(CommandAuditEntries.OccurredAt),
        AuditSource.App,
        NullLogger<UseCaseAuditRecorder>.Instance);
}
