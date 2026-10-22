using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_cancelled_ClassSession_is_restored;

public sealed class Then_the_cancellation_time_is_cleared
{
    [Fact]
    public void Then_the_cancellation_time_is_cleared_Run()
    {
        var session = ClassSession.Create(TestIds.Next(), TestData.Today, TestData.Now).WithId();
        session.Cancel("Feriado", TestData.Now);

        session.Restore();

        session.CancelledAt.ShouldBeNull();
    }
}
