using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Branches;

public sealed record ListBranchesQuery : IQuery;

public sealed class ListBranchesUseCase(IBranchDirectory branchDirectory, ICurrentUser currentUser, ITenantContext tenantContext)
    : IUseCase<ListBranchesQuery, IReadOnlyList<BranchResponse>>
{
    public async Task<Result<IReadOnlyList<BranchResponse>>> ExecuteAsync(ListBranchesQuery command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Result.Unauthorized<IReadOnlyList<BranchResponse>>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var branches = await branchDirectory.ListAsync(userId, cancellationToken);

        return Result.Success<IReadOnlyList<BranchResponse>>(
        [
            .. branches
                .OrderBy(branch => branch.BusinessName, StringComparer.CurrentCultureIgnoreCase)
                .Select(branch => BranchResponse.From(branch, tenantContext.TenantId)),
        ]);
    }
}
