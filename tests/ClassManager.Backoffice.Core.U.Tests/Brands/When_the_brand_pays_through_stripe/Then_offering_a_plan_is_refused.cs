using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_brand_pays_through_stripe;

public sealed class Then_offering_a_plan_is_refused
{
    [Fact]
    public async Task Then_offering_a_plan_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithStripeSubscription();

        var result = await scenario.OfferPlanAsync(PlanCodes.Lite);

        result.Error!.Kind.ShouldBe(ErrorKind.Conflict);
        result.Error.Code.ShouldBe(BrandErrorCodes.SubscriptionExternal);
        scenario.Repository.Offers.ShouldBeEmpty();
    }
}
