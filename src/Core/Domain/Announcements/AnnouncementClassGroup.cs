using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementClassGroup : ITenantOwned, IAuditable
{
    private AnnouncementClassGroup()
    {
    }

    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long AnnouncementId { get; private set; }
    public long ClassGroupId { get; private set; }

    internal static AnnouncementClassGroup Create(long announcementId, long classGroupId) =>
        new()
        {
            AnnouncementId = announcementId,
            ClassGroupId = classGroupId,
        };
}
