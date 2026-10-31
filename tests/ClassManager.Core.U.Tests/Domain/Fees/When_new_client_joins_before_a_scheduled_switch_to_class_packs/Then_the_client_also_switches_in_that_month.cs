using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_new_client_joins_before_a_scheduled_switch_to_class_packs;

public sealed class Then_the_client_also_switches_in_that_month
{
    [Fact]
    public void Then_the_client_also_switches_in_that_month_Run()
    {
        var defaultFeeChanges = new[]
        {
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.MonthlyFee, 25000m, TestData.Today, TestData.Now).Value!.WithId(),
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-11").Value!, BusinessBillingMode.MonthlyFee, 27000m, TestData.Today, TestData.Now).Value!.WithId(),
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-12").Value!, BusinessBillingMode.ClassPacks, 27000m, TestData.Today, TestData.Now).Value!.WithId(),
        };

        var planChanges = ClientBillingPlanChange.ForNewClient(TestData.Client(), defaultFeeChanges, TestData.Today, TestData.Now);

        planChanges.Select(change => (change.EffectiveFrom, change.Plan)).ShouldBe(
        [
            (new DateOnly(2026, 9, 1), BillingPlan.BusinessFee),
            (new DateOnly(2026, 12, 1), BillingPlan.ClassPacks),
        ]);
    }
}
