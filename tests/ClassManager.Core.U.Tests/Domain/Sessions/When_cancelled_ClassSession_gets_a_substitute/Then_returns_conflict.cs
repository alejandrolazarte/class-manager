using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_cancelled_ClassSession_gets_a_substitute;

public sealed class Then_returns_conflict
{
    [Fact]
    public void Then_returns_conflict_Run()
    {
        var session = ClassSession.Create(TestIds.Next(), TestData.Today, TestData.Now).WithId();
        session.Cancel("Feriado", TestData.Now);

        var result = session.AssignSubstitute(TestIds.Next(), TestIds.Next(), TestData.Now);

        result.Error!.Code.ShouldBe(SessionErrorCodes.Cancelled);
    }
}
