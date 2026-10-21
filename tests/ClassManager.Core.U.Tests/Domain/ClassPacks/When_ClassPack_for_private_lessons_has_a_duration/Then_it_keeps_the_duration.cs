namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_for_private_lessons_has_a_duration;

public sealed class Then_it_keeps_the_duration
{
    [Fact]
    public void Then_it_keeps_the_duration_Run()
    {
        var pack = ClassPackTestData.Pack();

        pack.DefineLessons([], 45);

        pack.ClassDurationMinutes.ShouldBe(45);
    }
}
