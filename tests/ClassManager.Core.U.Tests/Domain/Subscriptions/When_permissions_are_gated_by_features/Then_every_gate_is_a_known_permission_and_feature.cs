using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Core.U.Tests.Domain.Subscriptions.When_permissions_are_gated_by_features;

public sealed class Then_every_gate_is_a_known_permission_and_feature
{
    [Fact]
    public void Then_every_gate_is_a_known_permission_and_feature_Run()
    {
        var gates = FeatureGatedPermissions.FeatureByPermission;

        gates.ShouldNotBeEmpty();
        gates.Keys.ShouldAllBe(permission => Permissions.All.Contains(permission));
        gates.Values.ShouldAllBe(featureCode => Features.All.Contains(featureCode));
    }
}
