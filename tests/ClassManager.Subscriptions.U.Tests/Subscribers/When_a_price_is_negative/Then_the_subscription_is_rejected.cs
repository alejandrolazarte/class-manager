using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_price_is_negative;

public sealed class Then_the_subscription_is_rejected
{
    [Fact]
    public void Then_the_subscription_is_rejected_Run()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            Subscription.Start(SubscriberId, PlanCode, price: -1m, Currency, note: null, Now));
    }
}
