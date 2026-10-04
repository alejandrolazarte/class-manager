using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_description_is_too_long;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var pack = ClassPack.Create("8 clases", 8, 160m, 2, TestData.Now).Value!;

        var description = pack.Describe(new string('a', ClassPack.DescriptionMaxLength + 1));

        description.Error!.FieldName.ShouldBe(nameof(ClassPack.Description));
    }
}
