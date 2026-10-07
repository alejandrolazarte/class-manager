namespace ClassManager.Infrastructure.BackgroundTasks;

public interface IBackgroundTaskOutbox
{
    Task EnqueueAsync(BackgroundTaskCommand command, CancellationToken cancellationToken);
}
