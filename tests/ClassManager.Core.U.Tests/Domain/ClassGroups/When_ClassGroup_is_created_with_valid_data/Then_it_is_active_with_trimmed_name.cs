using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassGroup_is_created_with_valid_data;

public sealed class Then_it_is_active_with_trimmed_name
{
    [Fact]
    public void Then_it_is_active_with_trimmed_name_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Tuesday], "18:00", 45).Value!;

        var classGroup = ClassGroup.Create("  Natación inicial ", Guid.CreateVersion7(), schedule, 8, " ");

        classGroup.Value!.Name.ShouldBe("Natación inicial");
        classGroup.Value.IsActive.ShouldBeTrue();
        classGroup.Value.Location.ShouldBeNull();
    }
}
