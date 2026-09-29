namespace ClassManager.Api.Orders;

internal sealed partial class ExpiredOrderCancellationWorker(
    IExpiredOrderCancellationService cancellationService,
    TimeProvider timeProvider,
    ILogger<ExpiredOrderCancellationWorker> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval, timeProvider);
        do
        {
            try
            {
                await cancellationService.CancelExpiredOrdersAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogFailed(logger, exception);
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Cancelling unpaid orders failed")]
    private static partial void LogFailed(ILogger logger, Exception exception);
}
