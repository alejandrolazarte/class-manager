using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedules_share_a_weekday_and_overlap;

public sealed class Then_they_overlap
{
    [Fact]
    public void Then_they_overlap_Run()
    {
        var tuesdayAndThursday = ClassSchedule.Create([DayOfWeek.Tuesday, DayOfWeek.Thursday], "18:00", 45).Value!;
        var thursdayLater = ClassSchedule.Create([DayOfWeek.Thursday], "18:30", 60).Value!;

        tuesdayAndThursday.OverlapsWith(thursdayLater).ShouldBeTrue();
    }
}
