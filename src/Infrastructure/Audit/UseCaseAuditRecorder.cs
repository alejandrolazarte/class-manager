using System.Diagnostics;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.Audit;

public sealed partial class UseCaseAuditRecorder(
    ICommandAuditService auditLog,
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    AuditSource source,
    ILogger<UseCaseAuditRecorder> logger)
{
    public async Task<Result<TResponse>> RunAsync<TResponse>(object input, UseCaseStep<TResponse> nextStep)
    {
        var occurredAt = timeProvider.GetUtcNow();
        var startedAt = timeProvider.GetTimestamp();
        Result<TResponse> result;
        try
        {
            result = await nextStep();
        }
        catch (Exception exception)
        {
            Record(input, occurredAt, startedAt, CommandOutcome.Exception, exception.GetType().Name, errorCode: null, rowCount: null);
            throw;
        }

        if (result.Error is { } error)
        {
            Record(input, occurredAt, startedAt, CommandOutcome.Failed, error.Kind.ToString(), error.Code, rowCount: null);
        }
        else
        {
            Record(input, occurredAt, startedAt, CommandOutcome.Succeeded, errorKind: null, errorCode: null, (result.Value as ICountedRows)?.RowCount);
        }

        return result;
    }

    private void Record(object input, DateTimeOffset occurredAt, long startedAt, CommandOutcome outcome, string? errorKind, string? errorCode, int? rowCount)
    {
        try
        {
            var (payload, payloadTruncated) = CommandAuditPayload.Serialize(input);
            auditLog.Record(new CommandAuditEntry(
                occurredAt,
                CurrentTenantId(),
                input.GetType().Name,
                currentUser.AccountId,
                currentUser.UserId,
                outcome,
                errorKind,
                errorCode,
                (long)timeProvider.GetElapsedTime(startedAt).TotalMilliseconds,
                Activity.Current?.TraceId.ToString(),
                payload,
                payloadTruncated,
                rowCount,
                source.Name));
        }
        catch (Exception exception)
        {
            LogRecordFailed(logger, exception, input.GetType().Name);
        }
    }

    private Guid? CurrentTenantId()
    {
        try
        {
            return tenantContext.TenantId;
        }
        catch (TenantNotResolvedException)
        {
            return null;
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "The audit entry of {Command} could not be recorded")]
    private static partial void LogRecordFailed(ILogger logger, Exception exception, string command);
}
