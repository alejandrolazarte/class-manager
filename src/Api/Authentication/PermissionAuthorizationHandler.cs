using ClassManager.Core.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;

namespace ClassManager.Api.Authentication;

internal sealed class PermissionAuthorizationHandler(ICurrentMember currentMember) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var access = await currentMember.GetAccessAsync(CancellationToken.None);
        if (access is not null && (requirement.AnyOfPermissions.Count == 0 || requirement.AnyOfPermissions.Any(access.HasPermission)))
        {
            context.Succeed(requirement);
        }
    }
}
