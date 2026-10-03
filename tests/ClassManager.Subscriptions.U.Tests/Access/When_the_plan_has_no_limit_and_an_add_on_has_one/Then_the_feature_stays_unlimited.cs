using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_the_plan_has_no_limit_and_an_add_on_has_one;

public sealed class Then_the_feature_stays_unlimited
{
    [Fact]
    public void Then_the_feature_stays_unlimited_Run()
    {
        var features = Combine([Included(CountedFeatureCode)], [AddOn(CountedFeatureCode, Today, limit: PlanLimit)]);

        features.LimitOf(CountedFeatureCode).ShouldBeNull();
    }
}
