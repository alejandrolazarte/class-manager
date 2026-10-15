using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementClassGroup : ITenantOwned
{
    private AnnouncementClassGroup()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid AnnouncementId { get; private set; }
    public Guid ClassGroupId { get; private set; }

    internal static AnnouncementClassGroup Create(Guid announcementId, Guid classGroupId) =>
        new()
        {
            AnnouncementId = announcementId,
            ClassGroupId = classGroupId,
        };
}
