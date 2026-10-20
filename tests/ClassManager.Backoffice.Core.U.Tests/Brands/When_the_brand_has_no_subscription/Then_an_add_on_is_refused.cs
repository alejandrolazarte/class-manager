using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_brand_has_no_subscription;

public sealed class Then_an_add_on_is_refused
{
    [Fact]
    public async Task Then_an_add_on_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.Repository.BrandIds.Add(scenario.BrandId);

        var result = await scenario.GiveAddOnAsync(Features.Shop, limit: null);

        result.Error!.Kind.ShouldBe(ErrorKind.Conflict);
    }
}
