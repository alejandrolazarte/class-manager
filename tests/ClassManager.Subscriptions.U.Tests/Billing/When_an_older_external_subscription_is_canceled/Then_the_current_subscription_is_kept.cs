using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_an_older_external_subscription_is_canceled;

public sealed class Then_the_current_subscription_is_kept
{
    [Fact]
    public void Then_the_current_subscription_is_kept_Run()
    {
        var paying = Paying();

        var result = Sync(paying, External(id: OtherExternalSubscriptionId, status: ExternalSubscriptionStatus.Canceled, endedOn: Today));

        result.Outcome.ShouldBe(ExternalSubscriptionSyncOutcome.NotCurrent);
        paying.ExpiredOn.ShouldBeNull();
    }
}
