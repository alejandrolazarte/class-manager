using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record ListAccountsQuery(string? CurrentKind);

public sealed record AccountResponse(Guid BusinessId, string BusinessName, string Kind, bool IsCurrent);

public sealed class ListAccountsUseCase(
    IBranchDirectory branchDirectory,
    IFamilyDirectory familyDirectory,
    ICurrentUser currentUser,
    ITenantContext tenantContext)
    : IUseCase<ListAccountsQuery, IReadOnlyList<AccountResponse>>
{
    public async Task<Result<IReadOnlyList<AccountResponse>>> ExecuteAsync(ListAccountsQuery command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Result.Unauthorized<IReadOnlyList<AccountResponse>>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var currentKind = AccountKinds.IsFamily(command.CurrentKind) ? AccountKinds.Family : AccountKinds.Team;
        bool IsCurrent(Guid businessId, string kind) => businessId == tenantContext.TenantId && kind == currentKind;
        var branches = await branchDirectory.ListAsync(userId, cancellationToken);
        var families = await familyDirectory.ListAsync(userId, cancellationToken);

        return Result.Success<IReadOnlyList<AccountResponse>>(
        [
            .. branches
                .OrderBy(branch => branch.BusinessName, StringComparer.CurrentCultureIgnoreCase)
                .Select(branch => new AccountResponse(
                    branch.BusinessId, branch.BusinessName, AccountKinds.Team, IsCurrent(branch.BusinessId, AccountKinds.Team))),
            .. families
                .OrderBy(family => family.BusinessName, StringComparer.CurrentCultureIgnoreCase)
                .Select(family => new AccountResponse(
                    family.BusinessId, family.BusinessName, AccountKinds.Family, IsCurrent(family.BusinessId, AccountKinds.Family))),
        ]);
    }
}
