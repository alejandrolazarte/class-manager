using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassSchedule_has_no_weekdays;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var schedule = ClassSchedule.Create([], "18:00", 45);

        schedule.Error!.FieldName.ShouldBe(nameof(ClassSchedule.Weekdays));
    }
}
