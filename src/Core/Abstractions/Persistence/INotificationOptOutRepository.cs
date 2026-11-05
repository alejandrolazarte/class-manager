using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.Abstractions.Persistence;

public interface INotificationOptOutRepository
{
    Task<IReadOnlyList<NotificationOptOut>> ListAsync(Guid accountId, CancellationToken cancellationToken);

    Task<NotificationOptOut?> FindAsync(Guid accountId, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken);

    Task<IReadOnlySet<Guid>> ListOptedOutAsync(
        IReadOnlyCollection<Guid> accountIds, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken);

    void Add(NotificationOptOut optOut);

    void Remove(NotificationOptOut optOut);

    Task DeleteEveryOptOutOfAccountAsync(Guid accountId, CancellationToken cancellationToken);
}
