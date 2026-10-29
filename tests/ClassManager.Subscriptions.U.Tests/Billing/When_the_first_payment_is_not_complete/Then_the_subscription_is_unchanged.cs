using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_first_payment_is_not_complete;

public sealed class Then_the_subscription_is_unchanged
{
    [Fact]
    public void Then_the_subscription_is_unchanged_Run()
    {
        var trial = Trial();

        var result = Sync(trial, External(status: ExternalSubscriptionStatus.Incomplete));

        result.Outcome.ShouldBe(ExternalSubscriptionSyncOutcome.NotStarted);
        trial.IsDeleted.ShouldBeFalse();
    }
}
