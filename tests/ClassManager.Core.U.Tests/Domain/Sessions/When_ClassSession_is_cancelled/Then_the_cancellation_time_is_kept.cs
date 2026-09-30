using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_is_cancelled;

public sealed class Then_the_cancellation_time_is_kept
{
    [Fact]
    public void Then_the_cancellation_time_is_kept_Run()
    {
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);

        session.Cancel("Feriado", TestData.Now.AddHours(2));

        session.CancelledAt.ShouldBe(TestData.Now.AddHours(2));
    }
}
