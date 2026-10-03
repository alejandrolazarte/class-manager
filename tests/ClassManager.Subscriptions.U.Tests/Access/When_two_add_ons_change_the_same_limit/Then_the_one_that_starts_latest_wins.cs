using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_two_add_ons_change_the_same_limit;

public sealed class Then_the_one_that_starts_latest_wins
{
    private const int EarlierLimit = 10;
    private const int LaterLimit = 2;

    [Fact]
    public void Then_the_one_that_starts_latest_wins_Run()
    {
        var later = AddOn(CountedFeatureCode, Today.AddDays(-1), limit: LaterLimit, createdAt: Now.AddDays(-5));
        var earlier = AddOn(CountedFeatureCode, Today.AddDays(-10), limit: EarlierLimit, createdAt: Now);

        var features = Combine([Included(CountedFeatureCode, PlanLimit)], [later, earlier]);

        features.LimitOf(CountedFeatureCode).ShouldBe(LaterLimit);
    }
}
