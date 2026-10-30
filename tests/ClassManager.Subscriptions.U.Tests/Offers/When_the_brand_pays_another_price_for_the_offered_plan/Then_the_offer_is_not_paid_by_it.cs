using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Offers.When_the_brand_pays_another_price_for_the_offered_plan;

public sealed class Then_the_offer_is_not_paid_by_it
{
    [Fact]
    public void Then_the_offer_is_not_paid_by_it_Run()
    {
        var offer = LiteOffer();

        var result = Sync(Trial(), External(LitePlanCode, LitePrice));

        offer.IsPaidBy(result.Started!).ShouldBeFalse();
    }
}
