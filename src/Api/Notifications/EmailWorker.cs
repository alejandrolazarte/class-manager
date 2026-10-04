using ClassManager.Core.Abstractions.Email;
using ClassManager.Infrastructure.Email;

namespace ClassManager.Api.Notifications;

internal sealed partial class EmailWorker(
    EmailOutbox queue,
    IServiceScopeFactory scopeFactory,
    ILogger<EmailWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                if (message.BusinessId is { } businessId)
                {
                    scope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(businessId);
                }

                await scope.ServiceProvider.GetRequiredService<IEmailSender>().SendAsync(message, stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogNotSent(logger, exception, message.Subject);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Email not sent: {Subject}")]
    private static partial void LogNotSent(ILogger logger, Exception exception, string subject);
}
