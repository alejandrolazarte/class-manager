using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_business_charges_with_class_packs;

public sealed class Then_a_client_without_plan_pays_per_class
{
    [Fact]
    public void Then_a_client_without_plan_pays_per_class_Run()
    {
        var defaultFeeChanges = new[]
        {
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.ClassPacks, null, TestData.Today, TestData.Now).Value!.WithId(),
        };

        var plan = FeeTimeline.PlanIn([], defaultFeeChanges, BillingMonth.From(TestData.Today));

        plan.ShouldBe(BillingPlan.ClassPacks);
    }
}
