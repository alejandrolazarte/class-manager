using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_is_rescheduled_past_midnight;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var session = ClassSession.Create(TestIds.Next(), TestData.Today, TestData.Now).WithId();

        var reschedule = session.Reschedule(new TimeOnly(23, 30), durationMinutes: 45, usualStartTime: new TimeOnly(18, 0), TestData.Now);

        reschedule.Error!.Kind.ShouldBe(ErrorKind.Validation);
        session.RescheduledStartTime.ShouldBeNull();
    }
}
