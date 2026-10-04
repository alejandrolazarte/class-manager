using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Tenancy;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record ListAccountsQuery(string? CurrentKind);

public sealed record AccountResponse(Guid BusinessId, string BusinessName, string Kind, bool IsCurrent);

public sealed class ListAccountsUseCase(
    IBranchDirectory branchDirectory,
    IStudentAppDirectory studentAppDirectory,
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

        var currentKind = AccountKinds.IsStudent(command.CurrentKind) ? AccountKinds.Student : AccountKinds.Team;
        bool IsCurrent(Guid businessId, string kind) => businessId == tenantContext.TenantId && kind == currentKind;
        var branches = await branchDirectory.ListAsync(userId, cancellationToken);
        var students = await studentAppDirectory.ListAsync(userId, cancellationToken);

        return Result.Success<IReadOnlyList<AccountResponse>>(
        [
            .. branches
                .OrderBy(branch => branch.BusinessName, StringComparer.CurrentCultureIgnoreCase)
                .Select(branch => new AccountResponse(
                    branch.BusinessId, branch.BusinessName, AccountKinds.Team, IsCurrent(branch.BusinessId, AccountKinds.Team))),
            .. students
                .OrderBy(student => student.BusinessName, StringComparer.CurrentCultureIgnoreCase)
                .Select(student => new AccountResponse(
                    student.BusinessId, student.BusinessName, AccountKinds.Student, IsCurrent(student.BusinessId, AccountKinds.Student))),
        ]);
    }
}
