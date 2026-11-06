namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_sales_fee_has_more_than_two_decimals;

public sealed class Then_it_is_refused
{
    [Fact]
    public void Then_it_is_refused_Run()
    {
        var plan = Plan.Create("lite", 1, 19m, "EUR", BillingPeriod.Monthly);

        Should.Throw<ArgumentException>(() => plan.ChangeSalesFee(1.555m));
    }
}
