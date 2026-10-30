using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_plan_is_offered_at_no_price;

public sealed class Then_the_price_field_is_refused
{
    [Fact]
    public async Task Then_the_price_field_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();

        var result = await scenario.OfferPlanAsync(PlanCodes.Lite, price: 0m);

        result.Error!.Kind.ShouldBe(ErrorKind.Validation);
        result.Error.Code.ShouldBe(BrandErrorCodes.OfferPriceNotPositive);
        scenario.Repository.Offers.ShouldBeEmpty();
    }
}
