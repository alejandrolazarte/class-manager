using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_business_switches_to_a_monthly_fee_after_a_client_joined_with_class_packs;

public sealed class Then_the_client_keeps_paying_per_class
{
    [Fact]
    public void Then_the_client_keeps_paying_per_class_Run()
    {
        var chargingWithClassPacks = DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.ClassPacks, null, TestData.Today, TestData.Now).Value!.WithId();
        var planChanges = ClientBillingPlanChange.ForNewClient(TestData.Client(), [chargingWithClassPacks], TestData.Today, TestData.Now);
        var switchToMonthlyFee = DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-10").Value!, BusinessBillingMode.MonthlyFee, 25000m, TestData.Today, TestData.Now).Value!.WithId();

        var octoberPlan = FeeTimeline.PlanIn(planChanges, [chargingWithClassPacks, switchToMonthlyFee], BillingMonth.Parse("2026-10").Value!);

        octoberPlan.ShouldBe(BillingPlan.ClassPacks);
    }
}
