using ClassManager.Core.Abstractions.Security;
using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task AddMissingInEveryBranchAsync(
        Guid organizationId,
        IReadOnlyList<UserAccount> accounts,
        DateTimeOffset createdAt,
        CancellationToken cancellationToken)
    {
        var branchIds = await context.Businesses
            .Where(business => business.OrganizationId == organizationId)
            .Select(business => business.Id)
            .ToListAsync(cancellationToken);
        var accountIds = accounts.Select(account => (Guid?)account.UserId).ToList();
        var linkedUsers = await context.Users
            .IgnoreTenantFilter()
            .Where(user => branchIds.Contains(user.TenantId) && accountIds.Contains(user.AccountId))
            .Select(user => new { user.TenantId, user.AccountId })
            .ToListAsync(cancellationToken);
        foreach (var branchId in branchIds)
        {
            foreach (var account in accounts.Where(account => !linkedUsers.Any(user => user.TenantId == branchId && user.AccountId == account.UserId)))
            {
                await context.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                    INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [AccountId], [CreatedAt], [IsDeleted])
                    VALUES ({Guid.CreateVersion7()}, {branchId}, {account.FullName}, {account.Email}, {account.UserId}, {createdAt.ToUniversalTime()}, 0)
                    """,
                    cancellationToken);
            }
        }
    }

    public Task<Guid?> FindIdByAccountAsync(Guid businessId, Guid accountId, CancellationToken cancellationToken) =>
        context.Users
            .IgnoreTenantFilter()
            .Where(user => user.TenantId == businessId && user.AccountId == accountId)
            .Select(user => (Guid?)user.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, string>> ListFullNamesAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken) =>
        userIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await context.Users
                .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
                .Where(user => userIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, user => user.FullName, cancellationToken);

    public Task ChangeEmailInEveryBusinessByAccountAsync(Guid accountId, string email, CancellationToken cancellationToken) =>
        context.Users
            .IgnoreTenantFilter()
            .Where(user => user.AccountId == accountId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.Email, email), cancellationToken);
}
