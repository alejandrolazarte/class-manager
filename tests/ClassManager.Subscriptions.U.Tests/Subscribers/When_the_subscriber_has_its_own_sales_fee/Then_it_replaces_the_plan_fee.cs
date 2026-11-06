namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_the_subscriber_has_its_own_sales_fee;

public sealed class Then_it_replaces_the_plan_fee
{
    [Fact]
    public void Then_it_replaces_the_plan_fee_Run()
    {
        var plan = Plan.Create("pro", 1, 39m, "EUR", BillingPeriod.Monthly);
        plan.ChangeSalesFee(1m);
        var subscriberFee = SalesFeeOverride.Create(subscriberId: 7, percent: 5m, note: null, DateTimeOffset.UnixEpoch);

        SalesFees.PercentOf(plan, subscriberFee).ShouldBe(5m);
    }
}
