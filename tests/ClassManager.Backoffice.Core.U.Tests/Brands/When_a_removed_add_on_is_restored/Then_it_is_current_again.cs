using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_removed_add_on_is_restored;

public sealed class Then_it_is_current_again
{
    [Fact]
    public async Task Then_it_is_current_again_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        var addOn = scenario.WithAddOn(scenario.WithCurrentSubscription(PlanCodes.Pro), Features.Shop);
        await scenario.RemoveAddOnAsync(addOn.Id);

        var result = await scenario.RestoreAddOnAsync(addOn.Id);

        result.IsSuccess.ShouldBeTrue();
        addOn.IsDeleted.ShouldBeFalse();
    }
}
