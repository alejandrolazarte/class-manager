using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.WebPush;

internal sealed class SendPushTaskHandler(PushDispatcher dispatcher) : IBackgroundTaskHandler<SendPushTask>
{
    public Task HandleAsync(SendPushTask command, CancellationToken cancellationToken) =>
        dispatcher.DispatchAsync(command.Push, cancellationToken);
}
