using System.Text.Json;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.BackgroundTasks;

internal sealed class BackgroundTaskOutbox(AppDbContext context, BackgroundTaskTypes types, TimeProvider timeProvider) : IBackgroundTaskOutbox
{
    public async Task EnqueueAsync(BackgroundTaskCommand command, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(command, command.GetType(), BackgroundTaskJson.Options);
        context.BackgroundTasks.Add(BackgroundTask.Create(types.Of(command).TypeName, payload, command.TenantId, timeProvider.GetUtcNow()));
        await context.SaveChangesAsync(cancellationToken);
    }
}
