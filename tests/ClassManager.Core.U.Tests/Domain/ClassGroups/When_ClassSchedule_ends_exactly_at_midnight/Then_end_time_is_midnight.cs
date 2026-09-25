using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedule_ends_exactly_at_midnight;

public sealed class Then_end_time_is_midnight
{
    [Fact]
    public void Then_end_time_is_midnight_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Saturday], "23:00", 60);

        schedule.Value!.EndTime.ShouldBe(TimeOnly.MinValue);
    }
}
