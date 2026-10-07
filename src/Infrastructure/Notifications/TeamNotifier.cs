using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class TeamNotifier(AppDbContext context, PushPublisher publisher, TimeProvider timeProvider)
{
    public async Task NotifyAsync(IReadOnlyCollection<Guid> userIds, PushMessage message, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        foreach (var userId in userIds)
        {
            context.TeamNotifications.Add(TeamNotification.Create(userId, message.Title, message.Body, message.Url, now));
        }

        await context.SaveChangesAsync(cancellationToken);
        await publisher.PublishToMembersAsync(userIds, message, cancellationToken);
    }
}
