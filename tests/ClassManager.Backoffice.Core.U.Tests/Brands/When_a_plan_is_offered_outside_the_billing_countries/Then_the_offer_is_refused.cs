using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_plan_is_offered_outside_the_billing_countries;

public sealed class Then_the_offer_is_refused
{
    private const string ArgentinaCountryCode = "AR";

    [Fact]
    public async Task Then_the_offer_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();
        scenario.Repository.CountryCode = ArgentinaCountryCode;

        var result = await scenario.OfferPlanAsync(PlanCodes.Lite);

        result.Error!.Kind.ShouldBe(ErrorKind.Conflict);
        result.Error.Code.ShouldBe(BrandErrorCodes.BillingNotAvailable);
        scenario.Repository.Offers.ShouldBeEmpty();
    }
}
