using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Notifications;

public sealed class NotificationOptOut : ITenantOwned, IAuditable
{
    private NotificationOptOut()
    {
    }

    public long TenantId { get; private set; }
    public Guid AccountId { get; private set; }
    public NotificationCategory Category { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }

    public static NotificationOptOut Create(Guid accountId, NotificationCategory category, NotificationChannel channel) =>
        new()
        {
            AccountId = accountId,
            Category = category,
            Channel = channel,
        };
}
