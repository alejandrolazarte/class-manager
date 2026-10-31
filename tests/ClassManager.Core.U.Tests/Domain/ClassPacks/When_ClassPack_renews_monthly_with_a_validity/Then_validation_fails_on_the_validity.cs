using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_renews_monthly_with_a_validity;

public sealed class Then_validation_fails_on_the_validity
{
    [Fact]
    public void Then_validation_fails_on_the_validity_Run()
    {
        var plan = ClassPack.Create("Cuota mensual", 8, 25000m, 2, TestData.Now, renewsMonthly: true);

        plan.Error!.FieldName.ShouldBe(nameof(ClassPack.ValidityMonths));
    }
}
