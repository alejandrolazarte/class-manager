using ClassManager.Core.Domain.Achievements;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_student_attended_ten_classes;

public sealed class Then_class_medals_are_earned
{
    [Fact]
    public void Then_class_medals_are_earned_Run()
    {
        var medals = Medals.Earned(new MedalProgress(AttendedClasses: 10, Level: 1, BestStreakWeeks: 0), [], TestData.Today);

        medals.ShouldBe([Medal.FirstClass, Medal.TenClasses]);
    }
}
