using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_the_count_is_below_the_limit;

public sealed class Then_another_one_is_allowed
{
    [Fact]
    public void Then_another_one_is_allowed_Run()
    {
        var features = Combine([Included(CountedFeatureCode, PlanLimit)], []);

        features.AllowsAnother(CountedFeatureCode, currentCount: PlanLimit - 1).ShouldBeTrue();
    }
}
