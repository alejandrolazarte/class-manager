using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_subscription_starts_from_a_plan;

public sealed class Then_it_costs_the_list_price
{
    private const decimal ListPrice = 19m;

    [Fact]
    public void Then_it_costs_the_list_price_Run()
    {
        var plan = Plan.Create(PlanCode, displayOrder: 3, ListPrice, Currency, BillingPeriod.Monthly);

        var subscription = Subscription.StartAtListPrice(SubscriberId, plan, Today, Now);

        subscription.Price.ShouldBe(ListPrice);
    }
}
