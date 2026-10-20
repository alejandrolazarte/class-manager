using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_brand_has_an_add_on;

public sealed class Then_the_add_on_overrides_the_plan_limit
{
    [Fact]
    public async Task Then_the_add_on_overrides_the_plan_limit_Run()
    {
        var scenario = new BrandsScenario();
        var subscription = scenario.StartSubscription(PlanCodes.Pro, BrandsScenario.Now.AddDays(-5), price: 29m);
        scenario.WithBrand(
            [subscription],
            planFeatures: [PlanFeature.Create(PlanCodes.Pro, Features.Branches, 3), PlanFeature.Create(PlanCodes.Pro, Features.Shop)],
            addOns: [SubscriptionFeature.Create(subscription.Id, Features.Branches, 10m, BrandsScenario.Currency, 5, expiredOn: null, BrandsScenario.Now.AddDays(-1))]);

        var brand = (await scenario.GetAsync()).Value!;

        brand.Features.ShouldContain(new BrandFeatureResponse(Features.Branches, 5));
        brand.AddOns.ShouldHaveSingleItem().FeatureCode.ShouldBe(Features.Branches);
    }
}
