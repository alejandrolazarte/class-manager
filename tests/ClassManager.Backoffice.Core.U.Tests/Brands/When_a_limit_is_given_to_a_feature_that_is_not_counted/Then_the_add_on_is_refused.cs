using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_limit_is_given_to_a_feature_that_is_not_counted;

public sealed class Then_the_add_on_is_refused
{
    [Fact]
    public async Task Then_the_add_on_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription(PlanCodes.Pro);

        var result = await scenario.GiveAddOnAsync(Features.Shop, limit: 3);

        result.Error!.Code.ShouldBe(BrandErrorCodes.LimitOnUncountedFeature);
        scenario.Repository.AddOns.ShouldBeEmpty();
    }
}
