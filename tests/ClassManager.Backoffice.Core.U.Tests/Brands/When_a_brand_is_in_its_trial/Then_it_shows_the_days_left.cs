using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_brand_is_in_its_trial;

public sealed class Then_it_shows_the_days_left
{
    [Fact]
    public async Task Then_it_shows_the_days_left_Run()
    {
        var scenario = new BrandsScenario();
        var trial = scenario.StartSubscription(PlanCodes.Free, BrandsScenario.Now.AddDays(-19));
        trial.Expire(new DateOnly(2026, 10, 30));
        scenario.WithListedBrand(trial);

        var brand = await scenario.ListSingleAsync();

        brand.Status.ShouldBe(BrandStatus.Trial);
        brand.TrialDaysLeft.ShouldBe(11);
    }
}
