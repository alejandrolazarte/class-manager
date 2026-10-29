namespace ClassManager.Subscriptions.U.Tests.Billing.When_a_lookup_key_is_built_for_a_plan;

public sealed class Then_the_plan_code_is_read_back
{
    [Fact]
    public void Then_the_plan_code_is_read_back_Run()
    {
        var lookupKey = PlanPriceLookupKey.For("pro", "EUR");

        lookupKey.ShouldBe("pro-monthly-eur");
        PlanPriceLookupKey.PlanCodeOf(lookupKey).ShouldBe("pro");
    }
}
