using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_covers_any_group_class_with_a_duration;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var pack = ClassPackTestData.Pack();

        var lessons = pack.DefineLessons([], 45, coversAnyGroupClass: true);

        lessons.Error!.FieldName.ShouldBe(nameof(ClassPack.ClassDurationMinutes));
    }
}
