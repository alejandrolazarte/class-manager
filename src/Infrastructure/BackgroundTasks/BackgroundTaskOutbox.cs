using System.Text.Json;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.BackgroundTasks;

internal sealed class BackgroundTaskOutbox(AppDbContext context, BackgroundTaskTypes types, TimeProvider timeProvider) : IBackgroundTaskOutbox
{
    public Task EnqueueAsync(BackgroundTaskCommand command, CancellationToken cancellationToken) =>
        EnqueueAsync(command, timeProvider.GetUtcNow(), cancellationToken);

    public async Task EnqueueAsync(BackgroundTaskCommand command, DateTimeOffset runOn, CancellationToken cancellationToken)
    {
        var payload = PayloadOf(command);
        context.BackgroundTasks.Add(BackgroundTask.Create(types.Of(command).TypeName, payload, command.TenantId, timeProvider.GetUtcNow(), runOn));
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelPendingAsync(BackgroundTaskCommand command, CancellationToken cancellationToken)
    {
        var type = types.Of(command).TypeName;
        var payload = PayloadOf(command);
        var now = timeProvider.GetUtcNow();
        await context.BackgroundTasks
            .Where(task => task.Type == type && task.Payload == payload && task.Attempts == 0 && task.FailedOn == null && task.NextAttemptOn > now)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private static string PayloadOf(BackgroundTaskCommand command) =>
        JsonSerializer.Serialize(command, command.GetType(), BackgroundTaskJson.Options);
}
