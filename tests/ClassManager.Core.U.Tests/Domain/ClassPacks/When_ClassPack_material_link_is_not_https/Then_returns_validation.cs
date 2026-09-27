using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_material_link_is_not_https;

public sealed class Then_returns_validation
{
    [Fact]
    public void Then_returns_validation_Run()
    {
        var pack = ClassPack.Create("ADG De Autor", 10, 450m, 3, TestData.Now).Value!;

        var lessons = pack.DefineLessons(45, "http://example.com/adg.pdf");

        lessons.Error!.FieldName.ShouldBe(nameof(ClassPack.MaterialUrl));
    }
}
