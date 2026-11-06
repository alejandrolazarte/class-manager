using ClassManager.Tenancy.AspNetCore.Persistence;


namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class NotificationOptOutRepository(AppDbContext context) : INotificationOptOutRepository
{
    public async Task<IReadOnlyList<NotificationOptOut>> ListAsync(Guid accountId, CancellationToken cancellationToken) =>
        await context.NotificationOptOuts.Where(optOut => optOut.AccountId == accountId).ToListAsync(cancellationToken);

    public Task<NotificationOptOut?> FindAsync(
        Guid accountId, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken) =>
        context.NotificationOptOuts.FirstOrDefaultAsync(
            optOut => optOut.AccountId == accountId && optOut.Category == category && optOut.Channel == channel,
            cancellationToken);

    public async Task<IReadOnlySet<Guid>> ListOptedOutAsync(
        IReadOnlyCollection<Guid> accountIds, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken) =>
        accountIds.Count == 0
            ? new HashSet<Guid>()
            : (await context.NotificationOptOuts.AsNoTracking()
                .Where(optOut => accountIds.Contains(optOut.AccountId) && optOut.Category == category && optOut.Channel == channel)
                .Select(optOut => optOut.AccountId)
                .ToListAsync(cancellationToken))
                .ToHashSet();

    public void Add(NotificationOptOut optOut) => context.NotificationOptOuts.Add(optOut);

    public void Remove(NotificationOptOut optOut) => context.NotificationOptOuts.Remove(optOut);

    public Task DeleteEveryOptOutOfAccountAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.NotificationOptOuts.IgnoreTenantFilter().Where(optOut => optOut.AccountId == accountId).ExecuteDeleteAsync(cancellationToken);
}
