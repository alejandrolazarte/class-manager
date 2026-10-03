using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_subscription_expires;

public sealed class Then_it_is_not_active_the_next_day
{
    [Fact]
    public void Then_it_is_not_active_the_next_day_Run()
    {
        var subscription = Subscription.Start(SubscriberId, PlanCode, price: 0m, Currency, note: null, Now.AddMonths(-1));

        subscription.Expire(Today);

        subscription.IsActiveOn(Today.AddDays(1)).ShouldBeFalse();
        subscription.IsActiveOn(Today).ShouldBeTrue();
    }
}
