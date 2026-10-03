using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_plan_has_no_list_price;

public sealed class Then_the_subscription_starts_free
{
    [Fact]
    public void Then_the_subscription_starts_free_Run()
    {
        var plan = Plan.Create(PlanCode, displayOrder: 4, listPrice: null, Currency, BillingPeriod.Monthly);

        var subscription = Subscription.StartAtListPrice(SubscriberId, plan, Now);

        subscription.Price.ShouldBe(0m);
    }
}
