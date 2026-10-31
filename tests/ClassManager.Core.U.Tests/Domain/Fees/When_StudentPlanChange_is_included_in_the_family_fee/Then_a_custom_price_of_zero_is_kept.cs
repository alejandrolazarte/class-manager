using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_StudentPlanChange_is_included_in_the_family_fee;

public sealed class Then_a_custom_price_of_zero_is_kept
{
    [Fact]
    public void Then_a_custom_price_of_zero_is_kept_Run()
    {
        var monthlyPlan = ClassPack.Create("Cuota mensual", null, 25000m, null, TestData.Now, renewsMonthly: true).Value!.WithId();

        var change = StudentPlanChange.Create(TestIds.Next(), BillingMonth.From(TestData.Today), monthlyPlan, 0m, TestData.Today, TestData.Now);

        change.Value!.CustomPrice.ShouldBe(0m);
    }
}
