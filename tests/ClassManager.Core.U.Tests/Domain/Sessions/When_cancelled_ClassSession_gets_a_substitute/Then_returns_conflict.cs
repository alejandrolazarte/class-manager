using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_cancelled_ClassSession_gets_a_substitute;

public sealed class Then_returns_conflict
{
    [Fact]
    public void Then_returns_conflict_Run()
    {
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);
        session.Cancel("Feriado");

        var result = session.AssignSubstitute(Guid.CreateVersion7(), Guid.CreateVersion7());

        result.Error!.Code.ShouldBe(SessionErrorCodes.Cancelled);
    }
}
