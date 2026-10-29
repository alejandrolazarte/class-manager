using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_customer_cancels_at_period_end;

public sealed class Then_the_subscription_expires_on_the_last_paid_day
{
    [Fact]
    public void Then_the_subscription_expires_on_the_last_paid_day_Run()
    {
        var paying = Paying();
        var lastPaidDay = Today.AddDays(20);

        Sync(paying, External(cancelOn: lastPaidDay));

        paying.ExpiredOn.ShouldBe(lastPaidDay);
        paying.IsActiveOn(lastPaidDay).ShouldBeTrue();
    }
}
