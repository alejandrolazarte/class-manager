using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_subscription_has_expired;

public sealed class Then_the_brand_is_ended
{
    [Fact]
    public async Task Then_the_brand_is_ended_Run()
    {
        var scenario = new BrandsScenario();
        var trial = scenario.StartSubscription(PlanCodes.Free, BrandsScenario.Now.AddDays(-40));
        trial.Expire(new DateOnly(2026, 10, 10));
        scenario.WithListedBrand(trial);

        var brand = await scenario.ListSingleAsync();

        brand.Status.ShouldBe(BrandStatus.Ended);
        brand.TrialDaysLeft.ShouldBeNull();
    }
}
