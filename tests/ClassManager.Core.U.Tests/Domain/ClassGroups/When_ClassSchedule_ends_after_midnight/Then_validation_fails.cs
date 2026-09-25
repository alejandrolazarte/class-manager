using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedule_ends_after_midnight;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Friday], "23:30", 45);

        schedule.Error!.FieldName.ShouldBe(nameof(ClassSchedule.DurationMinutes));
    }
}
