using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_has_no_classes;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var pack = ClassPack.Create("Pack vacío", 0, 80m, null, TestData.Now);

        pack.Error!.FieldName.ShouldBe(nameof(ClassPack.ClassCount));
    }
}
