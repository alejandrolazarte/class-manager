using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Roles;

public sealed record ListRolesQuery;

public sealed class ListRolesUseCase(ICustomRoleRepository customRoleRepository, IBusinessMemberRepository businessMemberRepository)
    : IUseCase<ListRolesQuery, IReadOnlyList<RoleResponse>>
{
    private static readonly BusinessRole[] SystemRoles = [BusinessRole.BranchOwner, BusinessRole.Coach, BusinessRole.Viewer];

    public async Task<Result<IReadOnlyList<RoleResponse>>> ExecuteAsync(ListRolesQuery command, CancellationToken cancellationToken)
    {
        var members = await businessMemberRepository.ListAsync(cancellationToken);
        var customRoles = await customRoleRepository.ListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<RoleResponse>>(
        [
            .. SystemRoles.Select(role => RoleResponse.From(role, members.Count(member => member.Role == role))),
            .. customRoles
                .OrderBy(role => role.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(role => RoleResponse.From(role, members.Count(member => member.CustomRoleId == role.Id))),
        ]);
    }
}
