using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.WebPush;

internal sealed class SendPushBackgroundTaskHandler(PushDispatcher dispatcher) : IBackgroundTaskHandler<SendPushBackgroundTaskCommand>
{
    public Task HandleAsync(SendPushBackgroundTaskCommand command, CancellationToken cancellationToken) =>
        dispatcher.DispatchAsync(command.Push, cancellationToken);
}
