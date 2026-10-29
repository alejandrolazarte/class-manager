using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_a_payment_fails;

public sealed class Then_the_subscription_is_past_due_and_still_active
{
    [Fact]
    public void Then_the_subscription_is_past_due_and_still_active_Run()
    {
        var paying = Paying();

        Sync(paying, External(status: ExternalSubscriptionStatus.PastDue));

        paying.PaymentStatus.ShouldBe(SubscriptionPaymentStatus.PastDue);
        paying.IsActiveOn(Today).ShouldBeTrue();
    }
}
