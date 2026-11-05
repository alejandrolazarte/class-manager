using ClassManager.Core.Abstractions.Notifications;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class NotificationPreferencesService(INotificationOptOutRepository optOutRepository) : INotificationPreferencesService
{
    public async Task<IReadOnlyList<Guid>> WhoWantsAsync(
        IReadOnlyCollection<Guid> accountIds, NotificationCategory category, NotificationChannel channel, CancellationToken cancellationToken)
    {
        var optedOut = await optOutRepository.ListOptedOutAsync(accountIds, category, channel, cancellationToken);
        return [.. accountIds.Where(accountId => !optedOut.Contains(accountId))];
    }
}
