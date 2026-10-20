using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_for_group_classes_has_a_duration;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var pack = ClassPackTestData.Pack();

        var lessons = pack.DefineLessons([Guid.CreateVersion7()], 45);

        lessons.Error!.FieldName.ShouldBe(nameof(ClassPack.ClassDurationMinutes));
    }
}
