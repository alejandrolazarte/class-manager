using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedules_overlap_in_time_on_different_weekdays;

public sealed class Then_they_do_not_overlap
{
    [Fact]
    public void Then_they_do_not_overlap_Run()
    {
        var tuesday = ClassSchedule.Create([DayOfWeek.Tuesday], "18:00", 45).Value!;
        var wednesday = ClassSchedule.Create([DayOfWeek.Wednesday], "18:00", 45).Value!;

        tuesday.OverlapsWith(wednesday).ShouldBeFalse();
    }
}
