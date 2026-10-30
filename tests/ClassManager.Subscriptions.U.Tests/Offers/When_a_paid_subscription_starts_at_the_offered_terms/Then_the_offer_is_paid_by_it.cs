using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Offers.When_a_paid_subscription_starts_at_the_offered_terms;

public sealed class Then_the_offer_is_paid_by_it
{
    [Fact]
    public void Then_the_offer_is_paid_by_it_Run()
    {
        var offer = LiteOffer();

        var result = Sync(Trial(), External(LitePlanCode, OfferedLitePrice, ExternalSubscriptionStatus.Trialing));

        offer.IsPaidBy(result.Started!).ShouldBeTrue();
    }
}
