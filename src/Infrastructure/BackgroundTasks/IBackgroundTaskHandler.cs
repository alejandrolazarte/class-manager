namespace ClassManager.Infrastructure.BackgroundTasks;

public interface IBackgroundTaskHandler<in TCommand>
    where TCommand : BackgroundTaskCommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}
