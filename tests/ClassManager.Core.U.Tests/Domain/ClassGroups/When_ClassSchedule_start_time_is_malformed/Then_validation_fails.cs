using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedule_start_time_is_malformed;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Friday], "6 pm", 45);

        schedule.Error!.FieldName.ShouldBe(nameof(ClassSchedule.StartTime));
    }
}
