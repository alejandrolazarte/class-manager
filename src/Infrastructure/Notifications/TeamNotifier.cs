using ClassManager.Core.Abstractions.Security;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class TeamNotifier(AppDbContext context, PushPublisher publisher, IIdentityService identityService, TimeProvider timeProvider)
{
    public async Task NotifyAsync(IReadOnlyCollection<Guid> userIds, LocalizedPushMessage message, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var languagesByUser = (await identityService.ListAccountsAsync(userIds, cancellationToken))
            .ToDictionary(account => account.UserId, account => account.PreferredLanguage);
        foreach (var userId in userIds)
        {
            var userMessage = message.For(languagesByUser.GetValueOrDefault(userId));
            context.TeamNotifications.Add(TeamNotification.Create(userId, userMessage.Title, userMessage.Body, userMessage.Url, now));
        }

        await context.SaveChangesAsync(cancellationToken);
        await publisher.PublishToMembersAsync(userIds, message, cancellationToken);
    }
}
