using System.Threading.Channels;

namespace ClassManager.Infrastructure.BackgroundTasks;

public sealed class BackgroundTaskSignal(TimeProvider timeProvider)
{
    private readonly Channel<bool> _doorbell = Channel.CreateBounded<bool>(
        new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite, SingleReader = true });

    public void Ring() => _doorbell.Writer.TryWrite(true);

    public async Task WaitAsync(DateTimeOffset? until, CancellationToken cancellationToken)
    {
        if (until is null)
        {
            await _doorbell.Reader.ReadAsync(cancellationToken);
            return;
        }

        var delay = until.Value - timeProvider.GetUtcNow();
        if (delay <= TimeSpan.Zero)
        {
            return;
        }

        using var dueTime = new CancellationTokenSource(delay, timeProvider);
        using var waitEnd = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, dueTime.Token);
        try
        {
            await _doorbell.Reader.ReadAsync(waitEnd.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
        }
    }
}
