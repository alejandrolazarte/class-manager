using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit;

internal static class CommandAuditEntries
{
    public const string RecordPaymentCommand = "RecordPaymentCommand";
    public const string Payload = """{"clientId":"0199f6c4-1f1e-7000-8000-000000000001","amount":25000}""";

    public static readonly DateTimeOffset OccurredAt = new(2026, 10, 19, 18, 30, 0, TimeSpan.Zero);

    public static CommandAuditEntry Succeeded(Guid? tenantId, DateTimeOffset? occurredAt = null) => new(
        occurredAt ?? OccurredAt,
        tenantId,
        RecordPaymentCommand,
        AccountId: Guid.CreateVersion7(),
        UserId: Guid.CreateVersion7(),
        CommandOutcome.Succeeded,
        ErrorKind: null,
        ErrorCode: null,
        DurationMilliseconds: 42,
        TraceId: "4bf92f3577b34da6a3ce929d0e0e4736",
        Payload,
        PayloadTruncated: false);
}
