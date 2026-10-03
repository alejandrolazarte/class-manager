using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_plan_has_no_duration;

public sealed class Then_the_subscription_is_open_ended
{
    [Fact]
    public void Then_the_subscription_is_open_ended_Run()
    {
        var plan = Plan.Create(PlanCode, displayOrder: 3, 19m, Currency, BillingPeriod.Monthly);

        var subscription = Subscription.StartAtListPrice(SubscriberId, plan, Now);

        subscription.ExpiredOn.ShouldBeNull();
    }
}
