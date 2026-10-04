namespace ClassManager.Core.UseCases.StudentApp;

public enum StudentAppNewsKind
{
    Announcement,
    OrderReady,
    CoachFeedback,
    ClassCancelled,
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
    Guid? OrderId);

public sealed record StudentAppNewsResponse(IReadOnlyList<StudentAppNewsItemResponse> Items, int UnreadCount);
