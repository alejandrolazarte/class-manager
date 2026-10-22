using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class AnnouncementReply : ITenantOwned, IAuditable
{
    public const string NotAnEventCode = "announcement.not_an_event";

    private const string NotAnEventMessage = "Only events can be answered.";

    private AnnouncementReply()
    {
    }

    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long AnnouncementId { get; private set; }
    public long ClientId { get; private set; }
    public bool IsGoing { get; private set; }
    public DateTimeOffset RepliedAt { get; private set; }

    public static Result<AnnouncementReply> Create(Announcement announcement, long clientId, bool isGoing, DateTimeOffset repliedAt)
    {
        if (announcement.Category != AnnouncementCategory.Event)
        {
            return Result.Validation<AnnouncementReply>(NotAnEventMessage, NotAnEventCode);
        }

        var reply = new AnnouncementReply
        {
            AnnouncementId = announcement.Id,
            ClientId = clientId,
        };
        reply.Change(isGoing, repliedAt);
        return reply;
    }

    public void Change(bool isGoing, DateTimeOffset repliedAt)
    {
        IsGoing = isGoing;
        RepliedAt = repliedAt.ToUniversalTime();
    }
}
