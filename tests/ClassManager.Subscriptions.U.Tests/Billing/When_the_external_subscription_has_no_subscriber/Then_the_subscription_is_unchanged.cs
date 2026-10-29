using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_external_subscription_has_no_subscriber;

public sealed class Then_the_subscription_is_unchanged
{
    [Fact]
    public void Then_the_subscription_is_unchanged_Run()
    {
        var trial = Trial();

        var result = Sync(trial, External(subscriberId: null));

        result.Outcome.ShouldBe(ExternalSubscriptionSyncOutcome.MissingSubscriber);
        trial.IsDeleted.ShouldBeFalse();
    }
}
