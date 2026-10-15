using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementView : ITenantOwned
{
    private AnnouncementView()
    {
    }

    public Guid TenantId { get; private set; }
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
