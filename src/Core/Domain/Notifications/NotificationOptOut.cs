namespace ClassManager.Core.Domain.Notifications;

public sealed class NotificationOptOut
{
    private NotificationOptOut()
    {
    }

    public Guid AccountId { get; private set; }
    public NotificationCategory Category { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static NotificationOptOut Create(Guid accountId, NotificationCategory category, NotificationChannel channel, DateTimeOffset createdAt) =>
        new()
        {
            AccountId = accountId,
            Category = category,
            Channel = channel,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
