using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_ClassSession_is_rescheduled_to_its_usual_time;

public sealed class Then_the_override_is_cleared
{
    [Fact]
    public void Then_the_override_is_cleared_Run()
    {
        var session = ClassSession.Create(Guid.CreateVersion7(), TestData.Today, TestData.Now);
        session.Reschedule(new TimeOnly(19, 0), durationMinutes: 45, usualStartTime: new TimeOnly(18, 0));

        session.Reschedule(new TimeOnly(18, 0), durationMinutes: 45, usualStartTime: new TimeOnly(18, 0));

        session.RescheduledStartTime.ShouldBeNull();
    }
}
