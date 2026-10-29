using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_external_subscription_is_canceled;

public sealed class Then_the_subscription_ends_on_the_day_it_ended
{
    [Fact]
    public void Then_the_subscription_ends_on_the_day_it_ended_Run()
    {
        var paying = Paying();
        var endedOn = Today.AddDays(-1);

        Sync(paying, External(status: ExternalSubscriptionStatus.Canceled, endedOn: endedOn));

        paying.ExpiredOn.ShouldBe(endedOn);
        paying.IsActiveOn(Today).ShouldBeFalse();
    }
}
