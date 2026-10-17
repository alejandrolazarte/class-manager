using ClassManager.Core.Abstractions.Security;
using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user) => context.Users.Add(user);

    public Task<User?> FindByAccountForUpdateAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.Users.FirstOrDefaultAsync(user => user.AccountId == accountId, cancellationToken);

    public Task<bool> IsEmailTakenAsync(string email, Guid exceptUserId, CancellationToken cancellationToken) =>
        context.Users.AnyAsync(user => user.Email == email && user.Id != exceptUserId, cancellationToken);

    public Task<bool> IsEmailTakenByAnotherPersonOfTheAccountsBusinessesAsync(Guid accountId, string email, CancellationToken cancellationToken)
    {
        var businessIdsOfTheAccount = context.Users
            .IgnoreTenantFilter()
            .Where(user => user.AccountId == accountId)
            .Select(user => user.TenantId);
        return context.Users
            .IgnoreTenantFilter()
            .AnyAsync(
                user => businessIdsOfTheAccount.Contains(user.TenantId) && user.Email == email && user.AccountId != accountId,
                cancellationToken);
    }

    public async Task<IReadOnlySet<string>> ListEmailsAsync(CancellationToken cancellationToken) =>
        (await context.Users
            .Where(user => user.Email != null)
            .Select(user => user.Email!)
            .ToListAsync(cancellationToken))
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

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
                    SELECT {Guid.CreateVersion7()}, {branchId}, {account.FullName},
                        CASE WHEN EXISTS (
                            SELECT 1 FROM [Users]
                            WHERE [TenantId] = {branchId} AND [Email] = {account.Email} AND [IsDeleted] = 0) THEN NULL ELSE {account.Email} END,
                        {account.UserId}, {createdAt.ToUniversalTime()}, 0
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
            .Where(user => user.AccountId == accountId
                && !context.Users.IgnoreTenantFilter().Any(other => other.TenantId == user.TenantId && other.Id != user.Id && other.Email == email))
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.Email, email), cancellationToken);
}
