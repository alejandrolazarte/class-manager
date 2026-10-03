using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_neither_the_plan_nor_an_add_on_has_a_feature;

public sealed class Then_the_feature_is_not_available
{
    [Fact]
    public void Then_the_feature_is_not_available_Run()
    {
        var features = Combine([Included(IncludedFeatureCode)], [AddOn(AddOnFeatureCode)]);

        features.Has(MissingFeatureCode).ShouldBeFalse();
    }
}
