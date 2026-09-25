using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedule_duration_is_not_multiple_of_five;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Tuesday], "18:00", 47);

        schedule.Error!.FieldName.ShouldBe(nameof(ClassSchedule.DurationMinutes));
    }
}
