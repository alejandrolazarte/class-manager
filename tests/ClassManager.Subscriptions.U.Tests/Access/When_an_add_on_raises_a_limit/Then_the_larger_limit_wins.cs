using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_raises_a_limit;

public sealed class Then_the_larger_limit_wins
{
    private const int AddOnLimit = 5;

    [Fact]
    public void Then_the_larger_limit_wins_Run()
    {
        var features = Combine([Included(CountedFeatureCode, PlanLimit)], [AddOn(CountedFeatureCode, Today, limit: AddOnLimit)]);

        features.LimitOf(CountedFeatureCode).ShouldBe(AddOnLimit);
    }
}
