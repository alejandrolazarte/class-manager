using ClassManager.Core.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;

namespace ClassManager.Api.Authentication;

internal sealed class FamilyAuthorizationHandler(IFamilyAccess familyAccess) : AuthorizationHandler<FamilyRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FamilyRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true || !AccountKinds.IsFamily(SessionKindClaim.Of(context.User)))
        {
            return;
        }

        if (await familyAccess.GetAsync(CancellationToken.None) is not null)
        {
            context.Succeed(requirement);
        }
    }
}
