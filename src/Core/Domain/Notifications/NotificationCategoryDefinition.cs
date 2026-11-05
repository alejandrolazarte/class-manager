namespace ClassManager.Core.Domain.Notifications;

public sealed record NotificationCategoryDefinition(
    NotificationCategory Category,
    NotificationAudience Audience,
    IReadOnlyList<NotificationChannel> Channels)
{
    public bool Uses(NotificationChannel channel) => Channels.Contains(channel);
}
