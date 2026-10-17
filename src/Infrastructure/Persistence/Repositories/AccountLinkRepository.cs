using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class AccountLinkRepository(AppDbContext context) : IAccountLinkRepository
{
    public async Task UnlinkFromEveryBusinessAsync(Guid accountId, DateTimeOffset unlinkedAt, CancellationToken cancellationToken)
    {
        var deletedAt = unlinkedAt.ToUniversalTime();
        await context.BusinessMembers
            .IgnoreTenantFilter()
            .Where(member => member.User.AccountId == accountId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(member => member.IsDeleted, true)
                    .SetProperty(member => member.DeletedAt, deletedAt)
                    .SetProperty(member => member.DeletedByUserId, member => member.UserId),
                cancellationToken);
        await context.ClientAccounts
            .IgnoreTenantFilter()
            .Where(account => account.User.AccountId == accountId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(account => account.IsDeleted, true)
                    .SetProperty(account => account.DeletedAt, deletedAt)
                    .SetProperty(account => account.DeletedByUserId, account => account.UserId),
                cancellationToken);
        await context.OrganizationMembers
            .Where(member => member.UserId == accountId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(member => member.IsDeleted, true)
                    .SetProperty(member => member.DeletedAt, deletedAt),
                cancellationToken);
        await context.PushSubscriptions.IgnoreTenantFilter().Where(subscription => subscription.UserId == accountId).ExecuteDeleteAsync(cancellationToken);
        await context.MemberPushSubscriptions.IgnoreTenantFilter().Where(subscription => subscription.UserId == accountId).ExecuteDeleteAsync(cancellationToken);
        await context.TeamNotifications.IgnoreTenantFilter().Where(notification => notification.RecipientUserId == accountId).ExecuteDeleteAsync(cancellationToken);
        await context.Users
            .IgnoreQueryFilters([TenantModelBuilderExtensions.TenantQueryFilter, SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .Where(user => user.AccountId == accountId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.AccountId, (Guid?)null), cancellationToken);
    }
}
