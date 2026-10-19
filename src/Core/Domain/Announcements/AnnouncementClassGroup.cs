using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementClassGroup : ITenantOwned, IAuditable
{
    private AnnouncementClassGroup()
    {
    }

    public Guid TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public Guid AnnouncementId { get; private set; }
    public Guid ClassGroupId { get; private set; }

    internal static AnnouncementClassGroup Create(Guid announcementId, Guid classGroupId) =>
        new()
        {
            AnnouncementId = announcementId,
            ClassGroupId = classGroupId,
        };
}
