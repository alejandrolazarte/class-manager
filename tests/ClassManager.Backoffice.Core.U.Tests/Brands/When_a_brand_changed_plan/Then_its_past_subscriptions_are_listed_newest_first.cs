using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_brand_changed_plan;

public sealed class Then_its_past_subscriptions_are_listed_newest_first
{
    [Fact]
    public async Task Then_its_past_subscriptions_are_listed_newest_first_Run()
    {
        var scenario = new BrandsScenario();
        var trial = scenario.StartSubscription(PlanCodes.Free, BrandsScenario.Now.AddDays(-60));
        trial.Delete(BrandsScenario.Now.AddDays(-40));
        var lite = scenario.StartSubscription(PlanCodes.Lite, BrandsScenario.Now.AddDays(-40), price: 12m);
        lite.Delete(BrandsScenario.Now.AddDays(-10));
        var pro = scenario.StartSubscription(PlanCodes.Pro, BrandsScenario.Now.AddDays(-10), price: 29m);
        scenario.WithBrand([trial, lite, pro]);

        var brand = (await scenario.GetAsync()).Value!;

        brand.PastSubscriptions.Select(subscription => subscription.PlanCode).ShouldBe([PlanCodes.Lite, PlanCodes.Free]);
        brand.Subscription!.PlanCode.ShouldBe(PlanCodes.Pro);
    }
}
