namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_covers_any_group_class;

public sealed class Then_it_keeps_no_class_list
{
    [Fact]
    public void Then_it_keeps_no_class_list_Run()
    {
        var pack = ClassPackTestData.Pack();
        pack.DefineLessons([TestIds.Next()], null);

        pack.DefineLessons([TestIds.Next()], null, coversAnyGroupClass: true);

        pack.ClassGroupIds.ShouldBeEmpty();
    }
}
