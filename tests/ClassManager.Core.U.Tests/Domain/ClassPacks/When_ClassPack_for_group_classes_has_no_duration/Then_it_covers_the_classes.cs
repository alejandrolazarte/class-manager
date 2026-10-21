namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_for_group_classes_has_no_duration;

public sealed class Then_it_covers_the_classes
{
    [Fact]
    public void Then_it_covers_the_classes_Run()
    {
        var pack = ClassPackTestData.Pack();
        var classGroupId = Guid.CreateVersion7();

        pack.DefineLessons([classGroupId], null);

        pack.ClassGroupIds.ShouldBe([classGroupId]);
    }
}
