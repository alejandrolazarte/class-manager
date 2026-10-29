using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_a_late_payment_succeeds;

public sealed class Then_the_subscription_is_current_again
{
    [Fact]
    public void Then_the_subscription_is_current_again_Run()
    {
        var paying = Paying();
        paying.ChangePaymentStatus(SubscriptionPaymentStatus.PastDue);

        Sync(paying, External());

        paying.PaymentStatus.ShouldBe(SubscriptionPaymentStatus.Current);
    }
}
