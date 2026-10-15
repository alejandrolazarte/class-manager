using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.UseCases.StudentApp;

public enum StudentAppNewsKind
{
    Announcement,
    OrderReady,
    InstructorFeedback,
    ClassCancelled,
    ClassChanged,
}

public sealed record StudentAppNewsItemResponse(
    string Id,
    StudentAppNewsKind Kind,
    DateTimeOffset OccurredAt,
    bool IsUnread,
    string? Title,
    string? Body,
    IReadOnlyList<string> StudentFullNames,
    string? ClassName,
    string? InstructorFullName,
    DateOnly? ClassDate,
    Guid? OrderId,
    string? ClassStartTime = null)
{
    public Guid? AnnouncementId { get; init; }

    public AnnouncementCategory? Category { get; init; }

    public bool? IsGoing { get; init; }

    public bool IsPinned { get; init; }

    public string? ImageUrl { get; init; }

    public IReadOnlyList<string> AudienceClassNames { get; init; } = [];
}

public sealed record StudentAppNewsResponse(IReadOnlyList<StudentAppNewsItemResponse> Items, int UnreadCount);
