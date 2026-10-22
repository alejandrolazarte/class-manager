using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_for_private_lessons_has_no_duration;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var pack = ClassPackTestData.Pack();

        var lessons = pack.DefineLessons([], null);

        lessons.Error!.FieldName.ShouldBe(nameof(ClassPack.ClassDurationMinutes));
    }
}
