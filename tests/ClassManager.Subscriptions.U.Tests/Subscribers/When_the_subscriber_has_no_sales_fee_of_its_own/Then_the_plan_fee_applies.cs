namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_the_subscriber_has_no_sales_fee_of_its_own;

public sealed class Then_the_plan_fee_applies
{
    [Fact]
    public void Then_the_plan_fee_applies_Run()
    {
        var plan = Plan.Create("lite", 1, 19m, "EUR", BillingPeriod.Monthly);
        plan.ChangeSalesFee(1.5m);

        SalesFees.PercentOf(plan, subscriberFee: null).ShouldBe(1.5m);
    }
}
