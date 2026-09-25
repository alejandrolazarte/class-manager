using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassGroup_is_created_with_zero_capacity;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Tuesday], "18:00", 45).Value!;

        var classGroup = ClassGroup.Create("Natación inicial", Guid.CreateVersion7(), schedule, 0, null);

        classGroup.Error!.FieldName.ShouldBe(nameof(ClassGroup.Capacity));
    }
}
