using ClassManager.Backoffice.Core.Logs;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Infrastructure.Logs;

internal sealed class AuditLogDirectory(AppDbContext context) : IAuditLogDirectory
{
    public async Task<IReadOnlyList<long>> ListBranchIdsAsync(long brandId, CancellationToken cancellationToken) =>
        await context.Businesses.AsNoTracking()
            .Where(business => business.OrganizationId == brandId)
            .Select(business => business.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<long, LoggedBranch>> FindBranchesAsync(IReadOnlyCollection<long> branchIds, CancellationToken cancellationToken) =>
        branchIds.Count == 0
            ? new Dictionary<long, LoggedBranch>()
            : await context.Businesses.AsNoTracking()
                .Where(business => branchIds.Contains(business.Id))
                .Join(
                    context.Organizations,
                    business => business.OrganizationId,
                    organization => organization.Id,
                    (business, organization) => new LoggedBranch(business.Id, business.Name, organization.Id, organization.Name))
                .ToDictionaryAsync(branch => branch.BranchId, cancellationToken);

    public async Task<IReadOnlyDictionary<long, string>> FindUserNamesAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken) =>
        userIds.Count == 0
            ? new Dictionary<long, string>()
            : await context.Users.IgnoreTenantFilter().AsNoTracking()
                .Where(user => userIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, user => user.FullName, cancellationToken);
}
