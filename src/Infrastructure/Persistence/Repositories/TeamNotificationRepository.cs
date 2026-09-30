namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class TeamNotificationRepository(AppDbContext context) : ITeamNotificationRepository
{
    public async Task<IReadOnlyList<TeamNotification>> ListLatestAsync(Guid recipientUserId, int limit, CancellationToken cancellationToken) =>
        await context.TeamNotifications.AsNoTracking()
            .Where(notification => notification.RecipientUserId == recipientUserId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public Task<int> CountUnseenAsync(Guid recipientUserId, CancellationToken cancellationToken) =>
        context.TeamNotifications.CountAsync(
            notification => notification.RecipientUserId == recipientUserId && notification.SeenAt == null, cancellationToken);

    public async Task<IReadOnlyList<TeamNotification>> ListUnseenForUpdateAsync(Guid recipientUserId, CancellationToken cancellationToken) =>
        await context.TeamNotifications
            .Where(notification => notification.RecipientUserId == recipientUserId && notification.SeenAt == null)
            .ToListAsync(cancellationToken);
}
