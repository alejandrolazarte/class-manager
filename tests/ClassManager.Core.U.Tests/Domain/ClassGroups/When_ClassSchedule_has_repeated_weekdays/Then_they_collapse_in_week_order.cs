using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedule_has_repeated_weekdays;

public sealed class Then_they_collapse_in_week_order
{
    [Fact]
    public void Then_they_collapse_in_week_order_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Tuesday], "09:00", 60);

        schedule.Value!.Days.ShouldBe([DayOfWeek.Tuesday, DayOfWeek.Sunday]);
    }
}
