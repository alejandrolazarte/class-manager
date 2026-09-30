using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record GetCurrentMemberQuery;

public sealed record CurrentMemberResponse(
    Guid BusinessId,
    BusinessRole? BranchRole,
    Guid? InstructorId,
    bool IsBrandOwner,
    IReadOnlyList<string> Permissions,
    Guid? CustomRoleId = null,
    Guid? UserId = null,
    string? FullName = null)
{
    public static CurrentMemberResponse From(MemberAccess access, string? fullName = null) =>
        new(access.BusinessId, access.BranchRole, access.InstructorId, access.IsBrandOwner, [.. access.Permissions.Order(StringComparer.Ordinal)], access.CustomRoleId, access.UserId, fullName);
}

public sealed class GetCurrentMemberUseCase(ICurrentMember currentMember, IIdentityService identityService)
    : IUseCase<GetCurrentMemberQuery, CurrentMemberResponse>
{
    public async Task<Result<CurrentMemberResponse>> ExecuteAsync(GetCurrentMemberQuery command, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return Result.Unauthorized<CurrentMemberResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var accounts = await identityService.ListAccountsAsync([access.UserId], cancellationToken);
        return CurrentMemberResponse.From(access, accounts.Count > 0 ? accounts[0].FullName : null);
    }
}
