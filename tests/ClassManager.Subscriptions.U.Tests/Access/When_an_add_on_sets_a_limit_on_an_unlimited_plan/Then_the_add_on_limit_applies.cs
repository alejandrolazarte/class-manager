using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_sets_a_limit_on_an_unlimited_plan;

public sealed class Then_the_add_on_limit_applies
{
    [Fact]
    public void Then_the_add_on_limit_applies_Run()
    {
        var features = Combine([Included(CountedFeatureCode)], [AddOn(CountedFeatureCode, Today, limit: PlanLimit)]);

        features.LimitOf(CountedFeatureCode).ShouldBe(PlanLimit);
    }
}
