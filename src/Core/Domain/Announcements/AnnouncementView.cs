using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementView : ITenantOwned, IAuditable
{
    private AnnouncementView()
    {
    }

    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long AnnouncementId { get; private set; }
    public long ClientId { get; private set; }
    public DateTimeOffset SeenAt { get; private set; }

    public static AnnouncementView Create(long announcementId, long clientId, DateTimeOffset seenAt) =>
        new()
        {
            AnnouncementId = announcementId,
            ClientId = clientId,
            SeenAt = seenAt.ToUniversalTime(),
        };
}
