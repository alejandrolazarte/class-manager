using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedules_touch_at_the_boundary;

public sealed class Then_they_do_not_overlap
{
    [Fact]
    public void Then_they_do_not_overlap_Run()
    {
        var first = ClassSchedule.Create([DayOfWeek.Tuesday], "18:00", 45).Value!;
        var next = ClassSchedule.Create([DayOfWeek.Tuesday], "18:45", 45).Value!;

        first.OverlapsWith(next).ShouldBeFalse();
    }
}
