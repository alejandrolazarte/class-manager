using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.Services;

public sealed class AccessScopes(ICurrentMember currentMember) : IAccessScopes
{
    public async Task<InstructorScope> ForInstructorsAsync(string everyInstructorPermission, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return InstructorScope.Only(null);
        }

        return access.HasPermission(everyInstructorPermission)
            ? InstructorScope.EveryInstructor
            : InstructorScope.Only(access.InstructorId);
    }

    public async Task<ClientScope?> ForClientsAsync(CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return new ClientScope(null, Guid.Empty);
        }

        return access.HasPermission(Permissions.Students.ViewAll)
            ? null
            : new ClientScope(access.InstructorId, access.UserId);
    }
}
