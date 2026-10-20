using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_removed_add_on_was_replaced;

public sealed class Then_it_can_not_be_restored
{
    [Fact]
    public async Task Then_it_can_not_be_restored_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        var addOn = scenario.WithAddOn(scenario.WithCurrentSubscription(PlanCodes.Pro), Features.Branches, limit: 4);
        await scenario.GiveAddOnAsync(Features.Branches, limit: 6);

        var result = await scenario.RestoreAddOnAsync(addOn.Id);

        result.Error!.Kind.ShouldBe(ErrorKind.Conflict);
        addOn.IsDeleted.ShouldBeTrue();
    }
}
