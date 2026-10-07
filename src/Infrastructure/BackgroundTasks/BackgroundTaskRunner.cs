using System.Text.Json;
using ClassManager.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.BackgroundTasks;

internal sealed partial class BackgroundTaskRunner(
    IServiceScopeFactory scopeFactory,
    BackgroundTaskTypes types,
    TimeProvider timeProvider,
    ILogger<BackgroundTaskRunner> logger)
    : IBackgroundTaskRunner
{
    public const int MaxAttempts = 5;

    private const int BatchSize = 20;

    private static readonly TimeSpan ClaimDuration = TimeSpan.FromMinutes(5);

    public async Task<DateTimeOffset?> RunDueTasksAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Guid> dueTaskIds;
        do
        {
            dueTaskIds = await ListDueTaskIdsAsync(cancellationToken);
            foreach (var taskId in dueTaskIds)
            {
                await RunAsync(taskId, cancellationToken);
            }
        }
        while (dueTaskIds.Count == BatchSize);

        await using var scope = scopeFactory.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<AppDbContext>().BackgroundTasks
            .Where(task => task.FailedOn == null)
            .MinAsync(task => (DateTimeOffset?)task.NextAttemptOn, cancellationToken);
    }

    private async Task<IReadOnlyList<Guid>> ListDueTaskIdsAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        await using var scope = scopeFactory.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<AppDbContext>().BackgroundTasks
            .Where(task => task.FailedOn == null && task.NextAttemptOn <= now)
            .OrderBy(task => task.NextAttemptOn)
            .Select(task => task.Id)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);
    }

    private async Task RunAsync(Guid taskId, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = timeProvider.GetUtcNow();
        var claimedUntil = now + ClaimDuration;
        var claimed = await context.BackgroundTasks
            .Where(task => task.Id == taskId && task.FailedOn == null && task.NextAttemptOn <= now)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(task => task.Attempts, task => task.Attempts + 1)
                    .SetProperty(task => task.NextAttemptOn, claimedUntil),
                cancellationToken);
        if (claimed == 0)
        {
            return;
        }

        var backgroundTask = await context.BackgroundTasks.SingleAsync(task => task.Id == taskId, cancellationToken);
        try
        {
            await RunHandlerAsync(backgroundTask, cancellationToken);
            context.BackgroundTasks.Remove(backgroundTask);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            backgroundTask.RecordFailure(exception.Message, timeProvider.GetUtcNow(), MaxAttempts);
            if (backgroundTask.FailedOn is null)
            {
                LogRetrying(logger, exception, backgroundTask.Type, backgroundTask.Id, backgroundTask.Attempts, backgroundTask.NextAttemptOn);
            }
            else
            {
                LogFailed(logger, exception, backgroundTask.Type, backgroundTask.Id, backgroundTask.Attempts);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task RunHandlerAsync(BackgroundTask backgroundTask, CancellationToken cancellationToken)
    {
        var registration = types.Named(backgroundTask.Type);
        var command = (BackgroundTaskCommand)JsonSerializer.Deserialize(backgroundTask.Payload, registration.CommandType, BackgroundTaskJson.Options)!;
        await using var handlerScope = scopeFactory.CreateAsyncScope();
        if (backgroundTask.TenantId is { } tenantId)
        {
            handlerScope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(tenantId);
        }

        await registration.RunAsync(handlerScope.ServiceProvider, command, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Background task {Type} {TaskId} failed on attempt {Attempts}; next attempt on {NextAttemptOn}")]
    private static partial void LogRetrying(ILogger logger, Exception exception, string type, Guid taskId, int attempts, DateTimeOffset nextAttemptOn);

    [LoggerMessage(Level = LogLevel.Error, Message = "Background task {Type} {TaskId} failed after {Attempts} attempts and will not run again")]
    private static partial void LogFailed(ILogger logger, Exception exception, string type, Guid taskId, int attempts);
}
