using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_is_restored;

public sealed class Then_it_is_not_cancelled
{
    [Fact]
    public void Then_it_is_not_cancelled_Run()
    {
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);
        session.Cancel("Feriado", TestData.Now);

        session.Restore();

        session.IsCancelled.ShouldBeFalse();
        session.CancellationReason.ShouldBeNull();
    }
}
