using Microsoft.AspNetCore.Authorization;

namespace ClassManager.Api.Authentication;

internal sealed class PermissionRequirement(IReadOnlyList<string> anyOfPermissions) : IAuthorizationRequirement
{
    public IReadOnlyList<string> AnyOfPermissions { get; } = anyOfPermissions;
}
