using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;

namespace ClassManager.Infrastructure.Security;

internal sealed class BusinessTokenSubjectResolver(IBranchDirectory branchDirectory) : ITokenSubjectResolver
{
    public async Task<TokenSubject?> ResolveAsync(Guid userId, string email, Guid? tenantId, CancellationToken cancellationToken)
    {
        var branch = tenantId is { } businessId
            ? await branchDirectory.FindAsync(userId, businessId, cancellationToken)
            : await branchDirectory.FindDefaultAsync(userId, cancellationToken);

        return branch is null ? null : new TokenSubject(userId, email, branch.BusinessId, branch.RoleName);
    }
}
