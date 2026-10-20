using System.Security.Claims;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Security.Tokens;

namespace ClassManager.Backoffice.Api.Authorization;

internal static class PlatformPermissionEndpointConventionBuilderExtensions
{
    public static TBuilder RequirePlatformPermission<TBuilder>(this TBuilder builder, string permission)
        where TBuilder : IEndpointConventionBuilder =>
        builder.RequireAuthorization(policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context => Grants(context.User, permission)));

    private static bool Grants(ClaimsPrincipal user, string permission) =>
        Enum.TryParse<BusinessRole>(user.FindFirst(SecurityClaimTypes.Role)?.Value, out var role)
        && SystemRolePermissions.IsPlatformRole(role)
        && SystemRolePermissions.Of(role).Contains(permission);
}
