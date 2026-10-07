using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Api.I.Tests.BackgroundTasks.When_the_doorbell_rings_before_the_wait;

public sealed class Then_the_wait_ends_at_once
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task Then_the_wait_ends_at_once_Run()
    {
        var signal = new BackgroundTaskSignal(TimeProvider.System);
        signal.Ring();
        var wait = signal.WaitAsync(null, CancellationToken.None);

        await wait.WaitAsync(Timeout);
        wait.IsCompletedSuccessfully.ShouldBeTrue();
    }
}
