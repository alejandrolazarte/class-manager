using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_the_plan_includes_a_feature;

public sealed class Then_the_feature_is_available
{
    [Fact]
    public void Then_the_feature_is_available_Run()
    {
        var features = Combine([Included(IncludedFeatureCode)], []);

        features.Has(IncludedFeatureCode).ShouldBeTrue();
    }
}
