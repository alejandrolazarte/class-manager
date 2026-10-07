using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Api.BackgroundTasks;

internal sealed partial class BackgroundTaskWorker(
    IBackgroundTaskRunner runner,
    BackgroundTaskSignal signal,
    TimeProvider timeProvider,
    ILogger<BackgroundTaskWorker> logger)
    : BackgroundService
{
    private static readonly TimeSpan RecoveryDelay = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTimeOffset? nextDueOn;
            try
            {
                nextDueOn = await runner.RunDueTasksAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogPassFailed(logger, exception);
                nextDueOn = timeProvider.GetUtcNow() + RecoveryDelay;
            }

            await signal.WaitAsync(nextDueOn, stoppingToken);
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Running background tasks failed; trying again later")]
    private static partial void LogPassFailed(ILogger logger, Exception exception);
}
