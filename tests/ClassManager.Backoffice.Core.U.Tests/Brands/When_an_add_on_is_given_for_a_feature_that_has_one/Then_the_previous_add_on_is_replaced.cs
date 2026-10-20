using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_an_add_on_is_given_for_a_feature_that_has_one;

public sealed class Then_the_previous_add_on_is_replaced
{
    [Fact]
    public async Task Then_the_previous_add_on_is_replaced_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        var previous = scenario.WithAddOn(scenario.WithCurrentSubscription(PlanCodes.Pro), Features.Branches, limit: 4);

        var result = await scenario.GiveAddOnAsync(Features.Branches, limit: 6);

        previous.IsDeleted.ShouldBeTrue();
        scenario.Repository.AddOns.Where(addOn => !addOn.IsDeleted).ShouldHaveSingleItem().Id.ShouldBe(result.Value!.Id);
        result.Value.Limit.ShouldBe(6);
    }
}
