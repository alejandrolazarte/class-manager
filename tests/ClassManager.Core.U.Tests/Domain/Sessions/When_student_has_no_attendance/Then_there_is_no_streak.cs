using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_student_has_no_attendance;

public sealed class Then_there_is_no_streak
{
    [Fact]
    public void Then_there_is_no_streak_Run()
    {
        var streak = AttendanceStreaks.Calculate([], TestData.Today);

        streak.Weeks.ShouldBe(0);
        streak.Since.ShouldBeNull();
    }
}
