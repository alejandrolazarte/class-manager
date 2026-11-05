using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.Abstractions.Notifications;

public interface INotificationPreferencesService
{
    Task<IReadOnlyList<Guid>> WhoWantsAsync(
        IReadOnlyCollection<Guid> accountIds, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken);
}
