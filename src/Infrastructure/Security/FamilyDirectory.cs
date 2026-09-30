using ClassManager.Core.Abstractions.Security;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Security;

internal sealed class FamilyDirectory(AppDbContext context) : IFamilyDirectory
{
    public Task<FamilyLink?> FindDefaultAsync(Guid userId, CancellationToken cancellationToken) =>
        FirstAsync(userId, null, cancellationToken);

    public Task<FamilyLink?> FindAsync(Guid userId, Guid businessId, CancellationToken cancellationToken) =>
        FirstAsync(userId, businessId, cancellationToken);

    public async Task<IReadOnlyList<FamilyLink>> ListAsync(Guid userId, CancellationToken cancellationToken) =>
        await (
            from account in context.ClientAccounts.IgnoreQueryFilters().AsNoTracking()
            where account.UserId == userId
            join business in context.Businesses.AsNoTracking() on account.TenantId equals business.Id
            orderby business.Name
            select new FamilyLink(account.TenantId, business.Name, account.ClientId))
            .ToListAsync(cancellationToken);

    private async Task<FamilyLink?> FirstAsync(Guid userId, Guid? businessId, CancellationToken cancellationToken)
    {
        var accountLink = await context.ClientAccounts
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(account => account.UserId == userId && (businessId == null || account.TenantId == businessId))
            .OrderBy(account => account.CreatedAt)
            .Select(account => new { account.TenantId, account.ClientId })
            .FirstOrDefaultAsync(cancellationToken);
        if (accountLink is null)
        {
            return null;
        }

        var businessName = await context.Businesses
            .AsNoTracking()
            .Where(business => business.Id == accountLink.TenantId)
            .Select(business => business.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return businessName is null ? null : new FamilyLink(accountLink.TenantId, businessName, accountLink.ClientId);
    }
}
