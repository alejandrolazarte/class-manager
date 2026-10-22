using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.Announcements;

public sealed record AnnouncementClassGroupResponse(long Id, string Name);

public sealed record AnnouncementResponse(
    long Id,
    string Title,
    string? Body,
    DateTimeOffset PublishedAt,
    bool IsScheduled,
    AnnouncementCategory Category,
    bool IsPinned,
    DateOnly? VisibleUntil,
    IReadOnlyList<AnnouncementClassGroupResponse> ClassGroups,
    IReadOnlyList<CatalogImageResponse> Images,
    int RecipientCount,
    int SeenCount,
    int GoingCount,
    int NotGoingCount,
    AnnouncementAction Action,
    string? ActionUrl);

public sealed record AnnouncementRecipientResponse(long ClientId, string FullName, DateTimeOffset? SeenAt, bool? IsGoing);

public sealed record AnnouncementReminderResponse(int RemindedCount);
