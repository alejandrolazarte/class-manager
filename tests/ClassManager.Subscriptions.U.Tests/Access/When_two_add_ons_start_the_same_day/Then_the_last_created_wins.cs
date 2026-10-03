using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_two_add_ons_start_the_same_day;

public sealed class Then_the_last_created_wins
{
    private const int FirstLimit = 10;
    private const int CorrectedLimit = 4;

    [Fact]
    public void Then_the_last_created_wins_Run()
    {
        var corrected = AddOn(CountedFeatureCode, Today, limit: CorrectedLimit, createdAt: Now.AddHours(2));
        var first = AddOn(CountedFeatureCode, Today, limit: FirstLimit, createdAt: Now);

        var features = Combine([Included(CountedFeatureCode, PlanLimit)], [corrected, first]);

        features.LimitOf(CountedFeatureCode).ShouldBe(CorrectedLimit);
    }
}
