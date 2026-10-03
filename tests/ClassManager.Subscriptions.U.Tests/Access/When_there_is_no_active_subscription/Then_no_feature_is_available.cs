using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_there_is_no_active_subscription;

public sealed class Then_no_feature_is_available
{
    [Fact]
    public void Then_no_feature_is_available_Run()
    {
        var features = EffectiveFeatures.Inactive(PlanCode);

        features.IsActive.ShouldBeFalse();
        features.Has(IncludedFeatureCode).ShouldBeFalse();
        features.PlanCode.ShouldBe(PlanCode);
    }
}
