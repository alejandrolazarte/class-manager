using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_subscription_is_replaced;

public sealed class Then_it_is_no_longer_active
{
    [Fact]
    public void Then_it_is_no_longer_active_Run()
    {
        var subscription = Subscription.Start(SubscriberId, PlanCode, price: 0m, Currency, note: null, Now);

        subscription.Delete(Now);

        subscription.IsCurrent().ShouldBeFalse();
        subscription.IsActiveOn(Today).ShouldBeFalse();
    }
}
