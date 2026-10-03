using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_subscription_ends;

public sealed class Then_it_is_not_active_the_next_day
{
    [Fact]
    public void Then_it_is_not_active_the_next_day_Run()
    {
        var subscription = Subscription.Start(SubscriberId, PlanCode, price: 0m, Currency, Today.AddMonths(-1), note: null, Now);

        subscription.End(Today);

        subscription.IsActiveOn(Today.AddDays(1)).ShouldBeFalse();
        subscription.IsActiveOn(Today).ShouldBeTrue();
    }
}
