using ClassManager.Infrastructure.BackgroundTasks;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.BackgroundTasks.When_the_wait_has_a_due_time;

public sealed class Then_the_wait_ends_at_it
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan UntilDue = TimeSpan.FromMinutes(4);

    [Fact]
    public async Task Then_the_wait_ends_at_it_Run()
    {
        var timeProvider = new FakeTimeProvider(BusinessApiFactory.Now);
        var signal = new BackgroundTaskSignal(timeProvider);
        var wait = signal.WaitAsync(BusinessApiFactory.Now + UntilDue, CancellationToken.None);
        timeProvider.Advance(UntilDue - TimeSpan.FromSeconds(1));
        wait.IsCompleted.ShouldBeFalse();

        timeProvider.Advance(TimeSpan.FromSeconds(1));

        await wait.WaitAsync(Timeout);
        wait.IsCompletedSuccessfully.ShouldBeTrue();
    }
}
