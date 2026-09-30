using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_cancelled_ClassSession_is_restored;

public sealed class Then_the_cancellation_time_is_cleared
{
    [Fact]
    public void Then_the_cancellation_time_is_cleared_Run()
    {
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);
        session.Cancel("Feriado", TestData.Now);

        session.Restore();

        session.CancelledAt.ShouldBeNull();
    }
}
