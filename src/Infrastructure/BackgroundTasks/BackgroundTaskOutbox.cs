using System.Text.Json;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.BackgroundTasks;

internal sealed class BackgroundTaskOutbox(AppDbContext context, BackgroundTaskTypes types, TimeProvider timeProvider) : IBackgroundTaskOutbox
{
    public Task EnqueueAsync(BackgroundTaskCommand command, CancellationToken cancellationToken) =>
        EnqueueAsync(command, timeProvider.GetUtcNow(), cancellationToken);

    public async Task EnqueueAsync(BackgroundTaskCommand command, DateTimeOffset runOn, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(command, command.GetType(), BackgroundTaskJson.Options);
        context.BackgroundTasks.Add(BackgroundTask.Create(types.Of(command).TypeName, payload, command.TenantId, timeProvider.GetUtcNow(), runOn));
        await context.SaveChangesAsync(cancellationToken);
    }
}
