using ClassManager.Core.Abstractions.Security;
using ClassManager.Subscriptions.Access;
using Microsoft.AspNetCore.Authorization;

namespace ClassManager.Api.Authentication;

internal sealed class PermissionAuthorizationHandler(ICurrentMember currentMember, IFeatureAccess featureAccess)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true || !AccountKinds.IsTeam(SessionKindClaim.Of(context.User)))
        {
            return;
        }

        var access = await currentMember.GetAccessAsync(CancellationToken.None);
        if (access is null)
        {
            return;
        }

        if (requirement.AnyOfPermissions.Count == 0)
        {
            context.Succeed(requirement);
            return;
        }

        var grantedPermissions = requirement.AnyOfPermissions.Where(access.HasPermission).ToList();
        if (grantedPermissions.Count == 0)
        {
            return;
        }

        var gatedFeatureCodes = grantedPermissions
            .Select(permission => FeatureGatedPermissions.AnyFeatureByPermission.GetValueOrDefault(permission))
            .ToList();
        if (gatedFeatureCodes.Any(featureCodes => featureCodes is null))
        {
            context.Succeed(requirement);
            return;
        }

        var features = await featureAccess.GetCurrentAsync(CancellationToken.None);
        if (gatedFeatureCodes.SelectMany(featureCodes => featureCodes!).Any(features.Has))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail(features.IsActive ? new FeatureNotInPlanReason(this, gatedFeatureCodes[0]![0]) : new SubscriptionInactiveReason(this));
    }
}
