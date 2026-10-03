using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_changes_a_limit;

public sealed class Then_it_replaces_the_plan_limit
{
    private const int LowerLimit = 1;

    [Fact]
    public void Then_it_replaces_the_plan_limit_Run()
    {
        var features = Combine([Included(CountedFeatureCode, PlanLimit)], [AddOn(CountedFeatureCode, Today, limit: LowerLimit)]);

        features.LimitOf(CountedFeatureCode).ShouldBe(LowerLimit);
    }
}
