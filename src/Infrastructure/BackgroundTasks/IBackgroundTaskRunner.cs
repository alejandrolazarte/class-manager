namespace ClassManager.Infrastructure.BackgroundTasks;

public interface IBackgroundTaskRunner
{
    Task<DateTimeOffset?> RunDueTasksAsync(CancellationToken cancellationToken);
}
