using ClassManager.Infrastructure.WebPush;

namespace ClassManager.Api.Notifications;

internal sealed partial class FamilyPushWorker(
    FamilyPushOutbox queue,
    IServiceScopeFactory scopeFactory,
    ILogger<FamilyPushWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var push in queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                scope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(push.TenantId);
                await scope.ServiceProvider.GetRequiredService<FamilyPushDispatcher>().DispatchAsync(push, stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogFailed(logger, exception, push.TenantId);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Sending family push notifications of business {BusinessId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid businessId);
}
