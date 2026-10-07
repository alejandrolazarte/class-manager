using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.U.Tests.Domain.ClassGroups.When_ClassGroup_material_link_is_not_https;

public sealed class Then_returns_validation
{
    [Fact]
    public void Then_returns_validation_Run()
    {
        var schedule = ClassSchedule.Create([DayOfWeek.Monday], "18:00", 45).Value!;
        var classGroup = ClassGroup.Create("Natación adultos", Guid.CreateVersion7(), schedule, 8, null).Value!;

        var material = classGroup.ShareMaterial("http://example.com/adultos.pdf");

        material.Error!.FieldName.ShouldBe(nameof(ClassGroup.MaterialUrl));
    }
}
