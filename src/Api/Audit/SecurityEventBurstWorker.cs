using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.Audit;

internal sealed class SecurityEventBurstWorker(
    SecurityEventThrottle throttle,
    ISecurityEventService securityEvents,
    TimeProvider timeProvider)
    : BackgroundService
{
    public static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public void ReportBursts()
    {
        var now = timeProvider.GetUtcNow();
        foreach (var (ipAddress, count) in throttle.TakeBursts())
        {
            securityEvents.Record(new SecurityEvent(
                now,
                SecurityEventType.UnauthorizedBurst,
                TenantId: null,
                AccountId: null,
                UserId: null,
                Route: null,
                Method: null,
                StatusCode: null,
                ipAddress,
                UserAgent: null,
                count,
                TraceId: null));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Window, timeProvider);
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                ReportBursts();
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}
