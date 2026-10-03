using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_replaces_another;

public sealed class Then_only_the_new_limit_counts
{
    private const int PreviousLimit = 10;
    private const int NewLimit = 2;

    [Fact]
    public void Then_only_the_new_limit_counts_Run()
    {
        var previous = AddOn(CountedFeatureCode, PreviousLimit, createdOn: Now.AddDays(-5));
        previous.Delete(Now);
        var replacement = AddOn(CountedFeatureCode, NewLimit, createdOn: Now);

        var features = Combine([Included(CountedFeatureCode, PlanLimit)], [previous, replacement]);

        features.LimitOf(CountedFeatureCode).ShouldBe(NewLimit);
    }
}
