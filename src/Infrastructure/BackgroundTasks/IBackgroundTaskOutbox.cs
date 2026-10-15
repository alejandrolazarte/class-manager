namespace ClassManager.Infrastructure.BackgroundTasks;

public interface IBackgroundTaskOutbox
{
    Task EnqueueAsync(BackgroundTaskCommand command, CancellationToken cancellationToken);

    Task EnqueueAsync(BackgroundTaskCommand command, DateTimeOffset runOn, CancellationToken cancellationToken);
}
