using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_plan_is_offered;

public sealed class Then_the_subscription_does_not_change
{
    [Fact]
    public async Task Then_the_subscription_does_not_change_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        var trial = scenario.WithCurrentSubscription();

        var result = await scenario.OfferPlanAsync(PlanCodes.Lite);

        result.Value!.PaymentLink.ShouldBe(FakeBrandBillingLinks.PlanOfferPaymentLink);
        var offer = scenario.Repository.Offers.ShouldHaveSingleItem();
        offer.PlanCode.ShouldBe(PlanCodes.Lite);
        offer.Price.ShouldBe(SubscriptionChangeScenario.OfferedPrice);
        trial.IsDeleted.ShouldBeFalse();
        scenario.Repository.Subscriptions.ShouldHaveSingleItem();
    }
}
