namespace ClassManager.Core.UseCases.Families;

public enum FamilyNewsKind
{
    Announcement,
    OrderReady,
    CoachFeedback,
    ClassCancelled,
}

public sealed record FamilyNewsItemResponse(
    string Id,
    FamilyNewsKind Kind,
    DateTimeOffset OccurredAt,
    bool IsUnread,
    string? Title,
    string? Body,
    IReadOnlyList<string> StudentFullNames,
    string? ClassName,
    string? InstructorFullName,
    DateOnly? ClassDate,
    Guid? OrderId);

public sealed record FamilyNewsResponse(IReadOnlyList<FamilyNewsItemResponse> Items, int UnreadCount);
