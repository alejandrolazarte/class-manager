using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementView : ITenantOwned, IAuditable
{
    private AnnouncementView()
    {
    }

    public Guid TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public Guid AnnouncementId { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTimeOffset SeenAt { get; private set; }

    public static AnnouncementView Create(Guid announcementId, Guid clientId, DateTimeOffset seenAt) =>
        new()
        {
            AnnouncementId = announcementId,
            ClientId = clientId,
            SeenAt = seenAt.ToUniversalTime(),
        };
}
