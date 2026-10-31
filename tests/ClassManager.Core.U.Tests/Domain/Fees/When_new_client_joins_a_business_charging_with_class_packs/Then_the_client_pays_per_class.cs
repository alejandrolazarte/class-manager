using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_new_client_joins_a_business_charging_with_class_packs;

public sealed class Then_the_client_pays_per_class
{
    [Fact]
    public void Then_the_client_pays_per_class_Run()
    {
        var client = TestData.Client();
        var defaultFeeChanges = new[]
        {
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.ClassPacks, null, TestData.Today, TestData.Now).Value!.WithId(),
        };

        var planChanges = ClientBillingPlanChange.ForNewClient(client, defaultFeeChanges, TestData.Today, TestData.Now);

        planChanges.Single().Plan.ShouldBe(BillingPlan.ClassPacks);
        planChanges.Single().EffectiveFrom.ShouldBe(new DateOnly(2026, 9, 1));
        planChanges.Single().Client.ShouldBeSameAs(client);
    }
}
