using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ClassManager.Api.Authentication;

internal sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(AuthorizationPolicies.PermissionPrefix, StringComparison.Ordinal))
        {
            return await base.GetPolicyAsync(policyName);
        }

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(AuthorizationPolicies.PermissionsOf(policyName)))
            .Build();
    }
}
