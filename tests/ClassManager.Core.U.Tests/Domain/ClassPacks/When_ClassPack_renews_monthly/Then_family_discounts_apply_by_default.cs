using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPack_renews_monthly;

public sealed class Then_family_discounts_apply_by_default
{
    [Fact]
    public void Then_family_discounts_apply_by_default_Run()
    {
        var plan = ClassPack.Create("2 veces por semana", 8, 25000m, null, TestData.Now, renewsMonthly: true);

        plan.Value!.AppliesFamilyDiscount.ShouldBeTrue();
    }
}
