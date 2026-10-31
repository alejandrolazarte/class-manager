using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_bought_once_has_libre_classes;

public sealed class Then_validation_fails_on_the_class_count
{
    [Fact]
    public void Then_validation_fails_on_the_class_count_Run()
    {
        var pack = ClassPack.Create("Pack libre", null, 25000m, null, TestData.Now);

        pack.Error!.FieldName.ShouldBe(nameof(ClassPack.ClassCount));
    }
}
