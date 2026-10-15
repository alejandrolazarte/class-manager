using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Announcements;

internal sealed record AnnouncementContent(string Title, string? Body, AnnouncementCategory Category, DateOnly? VisibleUntil)
{
    private const string TitleRequiredMessage = "Write a title.";
    private const string TitleLengthMessage = "The title must be at most 80 characters.";
    private const string BodyLengthMessage = "The text must be at most 1000 characters.";
    private const string CategoryMessage = "Choose the kind of announcement.";
    private const string VisibleUntilMessage = "The announcement can't be hidden before today.";

    public static Result<AnnouncementContent> From(
        string? title, string? body, AnnouncementCategory category, DateOnly? visibleUntil, DateOnly today)
    {
        var trimmedTitle = title?.Trim() ?? string.Empty;
        if (trimmedTitle.Length == 0)
        {
            return Result.Validation<AnnouncementContent>(TitleRequiredMessage, fieldName: nameof(Announcement.Title));
        }

        if (trimmedTitle.Length > Announcement.TitleMaxLength)
        {
            return Result.Validation<AnnouncementContent>(TitleLengthMessage, fieldName: nameof(Announcement.Title));
        }

        var trimmedBody = string.IsNullOrWhiteSpace(body) ? null : body.Trim();
        if (trimmedBody?.Length > Announcement.BodyMaxLength)
        {
            return Result.Validation<AnnouncementContent>(BodyLengthMessage, fieldName: nameof(Announcement.Body));
        }

        if (!Enum.IsDefined(category))
        {
            return Result.Validation<AnnouncementContent>(CategoryMessage, fieldName: nameof(Announcement.Category));
        }

        if (visibleUntil < today)
        {
            return Result.Validation<AnnouncementContent>(VisibleUntilMessage, fieldName: nameof(Announcement.VisibleUntil));
        }

        return new AnnouncementContent(trimmedTitle, trimmedBody, category, visibleUntil);
    }
}
