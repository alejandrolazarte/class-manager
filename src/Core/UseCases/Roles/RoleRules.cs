using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Roles;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Core.UseCases.Roles;

internal static class RoleRules
{
    public static async Task<ResultError?> CheckCanGrantAsync(ICurrentMember currentMember, CustomRole role, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        return access is not null && role.Permissions.All(access.HasPermission) ? null : MemberRules.ExceedsOwn();
    }
}
