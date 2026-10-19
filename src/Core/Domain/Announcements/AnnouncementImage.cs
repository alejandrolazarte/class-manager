using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementImage : ITenantOwned, IAuditable, ICatalogImageLink
{
    private AnnouncementImage()
    {
    }

    public Guid TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public Guid AnnouncementId { get; private set; }
    public Guid DocumentId { get; private set; }
    public Document Document { get; private set; } = null!;
    public int Position { get; private set; }

    public void MoveTo(int position) => Position = position;

    internal static AnnouncementImage Create(Guid announcementId, Document document, int position) =>
        new()
        {
            AnnouncementId = announcementId,
            DocumentId = document.Id,
            Document = document,
            Position = position,
        };
}
