using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_validity_is_longer_than_two_years;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var pack = ClassPack.Create("8 clases", 8, 160m, 25, TestData.Now);

        pack.Error!.FieldName.ShouldBe(nameof(ClassPack.ValidityMonths));
    }
}
