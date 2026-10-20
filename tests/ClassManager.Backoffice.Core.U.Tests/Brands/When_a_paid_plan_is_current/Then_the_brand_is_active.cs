using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_paid_plan_is_current;

public sealed class Then_the_brand_is_active
{
    [Fact]
    public async Task Then_the_brand_is_active_Run()
    {
        var scenario = new BrandsScenario();
        scenario.WithListedBrand(scenario.StartSubscription(PlanCodes.Pro, BrandsScenario.Now.AddDays(-5), price: 29m));

        var brand = await scenario.ListSingleAsync();

        brand.Status.ShouldBe(BrandStatus.Active);
        brand.PlanCode.ShouldBe(PlanCodes.Pro);
    }
}
