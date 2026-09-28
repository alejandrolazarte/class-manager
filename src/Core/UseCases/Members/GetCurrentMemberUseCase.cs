using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record GetCurrentMemberQuery;

public sealed record CurrentMemberResponse(
    Guid BusinessId,
    BusinessRole? BranchRole,
    bool IsBrandOwner,
    IReadOnlyList<string> Permissions)
{
    public static CurrentMemberResponse From(MemberAccess access) =>
        new(access.BusinessId, access.BranchRole, access.IsBrandOwner, [.. access.Permissions.Order(StringComparer.Ordinal)]);
}

public sealed class GetCurrentMemberUseCase(ICurrentMember currentMember) : IUseCase<GetCurrentMemberQuery, CurrentMemberResponse>
{
    public async Task<Result<CurrentMemberResponse>> ExecuteAsync(GetCurrentMemberQuery command, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);

        return access is null
            ? Result.Unauthorized<CurrentMemberResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess)
            : CurrentMemberResponse.From(access);
    }
}
