using ClassManager.Core.Abstractions.Security;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Security;

internal sealed class CurrentFamily(AppDbContext context, ICurrentUser currentUser, ITenantContext tenantContext) : IFamilyAccess
{
    private FamilyAccess? _resolvedAccess;
    private bool _isResolved;

    public async Task<FamilyAccess?> GetAsync(CancellationToken cancellationToken)
    {
        if (!_isResolved)
        {
            _resolvedAccess = await ResolveAsync(cancellationToken);
            _isResolved = true;
        }

        return _resolvedAccess;
    }

    private async Task<FamilyAccess?> ResolveAsync(CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return null;
        }

        var businessId = tenantContext.TenantId;
        var clientId = await context.ClientAccounts
            .AsNoTracking()
            .Where(account => account.UserId == userId)
            .Select(account => (Guid?)account.ClientId)
            .FirstOrDefaultAsync(cancellationToken);

        return clientId is { } linkedClientId ? new FamilyAccess(userId, businessId, linkedClientId) : null;
    }
}
