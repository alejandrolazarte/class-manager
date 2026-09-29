using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;

namespace ClassManager.Infrastructure.Security;

internal sealed class BusinessTokenSubjectResolver(IBranchDirectory branchDirectory, IFamilyDirectory familyDirectory) : ITokenSubjectResolver
{
    public async Task<TokenSubject?> ResolveAsync(Guid userId, string email, Guid? tenantId, string? kind, CancellationToken cancellationToken) =>
        AccountKinds.IsFamily(kind)
            ? await ResolveFamilyAsync(userId, email, tenantId, cancellationToken)
            : await ResolveTeamAsync(userId, email, tenantId, cancellationToken);

    private async Task<TokenSubject?> ResolveTeamAsync(Guid userId, string email, Guid? tenantId, CancellationToken cancellationToken)
    {
        var branch = tenantId is { } businessId
            ? await branchDirectory.FindAsync(userId, businessId, cancellationToken)
            : await branchDirectory.FindDefaultAsync(userId, cancellationToken);

        return branch is null ? null : new TokenSubject(userId, email, branch.BusinessId, branch.RoleName, AccountKinds.Team);
    }

    private async Task<TokenSubject?> ResolveFamilyAsync(Guid userId, string email, Guid? tenantId, CancellationToken cancellationToken)
    {
        var family = tenantId is { } businessId
            ? await familyDirectory.FindAsync(userId, businessId, cancellationToken)
            : await familyDirectory.FindDefaultAsync(userId, cancellationToken);

        return family is null
            ? null
            : new TokenSubject(userId, email, family.BusinessId, AccountKinds.FamilyRoleName, AccountKinds.Family);
    }
}
