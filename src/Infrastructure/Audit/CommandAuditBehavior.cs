using System.Diagnostics;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.Audit;

public sealed partial class CommandAuditBehavior<TCommand, TResponse>(
    ICommandAuditService auditLog,
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    ILogger<CommandAuditBehavior<TCommand, TResponse>> logger)
    : IUseCaseBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    public async Task<Result<TResponse>> HandleAsync(TCommand command, UseCaseStep<TResponse> nextStep, CancellationToken cancellationToken)
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
            Record(command, occurredAt, startedAt, CommandOutcome.Exception, exception.GetType().Name, errorCode: null);
            throw;
        }

        if (result.Error is { } error)
        {
            Record(command, occurredAt, startedAt, CommandOutcome.Failed, error.Kind.ToString(), error.Code);
        }
        else
        {
            Record(command, occurredAt, startedAt, CommandOutcome.Succeeded, errorKind: null, errorCode: null);
        }

        return result;
    }

    private void Record(TCommand command, DateTimeOffset occurredAt, long startedAt, CommandOutcome outcome, string? errorKind, string? errorCode)
    {
        try
        {
            var (payload, payloadTruncated) = CommandAuditPayload.Serialize(command);
            auditLog.Record(new CommandAuditEntry(
                occurredAt,
                CurrentTenantId(),
                typeof(TCommand).Name,
                currentUser.AccountId,
                currentUser.UserId,
                outcome,
                errorKind,
                errorCode,
                (long)timeProvider.GetElapsedTime(startedAt).TotalMilliseconds,
                Activity.Current?.TraceId.ToString(),
                payload,
                payloadTruncated));
        }
        catch (Exception exception)
        {
            LogRecordFailed(logger, exception, typeof(TCommand).Name);
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
