using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_price_has_an_unknown_lookup_key;

public sealed class Then_the_subscription_is_unchanged
{
    [Fact]
    public void Then_the_subscription_is_unchanged_Run()
    {
        var trial = Trial();

        var result = Sync(trial, External(lookupKey: "premium-monthly-eur"));

        result.Outcome.ShouldBe(ExternalSubscriptionSyncOutcome.UnknownPrice);
        trial.IsDeleted.ShouldBeFalse();
    }
}
