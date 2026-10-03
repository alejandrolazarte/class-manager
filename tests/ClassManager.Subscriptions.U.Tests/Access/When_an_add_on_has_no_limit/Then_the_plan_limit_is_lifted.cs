using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_has_no_limit;

public sealed class Then_the_plan_limit_is_lifted
{
    [Fact]
    public void Then_the_plan_limit_is_lifted_Run()
    {
        var features = Combine([Included(CountedFeatureCode, PlanLimit)], [AddOn(CountedFeatureCode, Today, limit: null)]);

        features.LimitOf(CountedFeatureCode).ShouldBeNull();
    }
}
