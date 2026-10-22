using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_business_switches_to_class_packs_in_october;

public sealed class Then_september_keeps_the_business_fee
{
    [Fact]
    public void Then_september_keeps_the_business_fee_Run()
    {
        var defaultFeeChanges = new[]
        {
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-10").Value!, BusinessBillingMode.ClassPacks, 25000m, TestData.Today, TestData.Now).Value!.WithId(),
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.MonthlyFee, 25000m, TestData.Today, TestData.Now).Value!.WithId(),
        };

        var septemberPlan = FeeTimeline.PlanIn([], defaultFeeChanges, BillingMonth.Parse("2026-09").Value!);

        septemberPlan.ShouldBe(BillingPlan.BusinessFee);
        FeeTimeline.PlanIn([], defaultFeeChanges, BillingMonth.Parse("2026-10").Value!).ShouldBe(BillingPlan.ClassPacks);
    }
}
