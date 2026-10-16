using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Announcements;

internal sealed record AnnouncementContent(
    string Title,
    string? Body,
    AnnouncementCategory Category,
    DateOnly? VisibleUntil,
    AnnouncementAction Action,
    string? ActionUrl)
{
    private const string TitleRequiredMessage = "Write a title.";
    private const string TitleLengthMessage = "The title must be at most 80 characters.";
    private const string BodyLengthMessage = "The text must be at most 1000 characters.";
    private const string CategoryMessage = "Choose the kind of announcement.";
    private const string VisibleUntilMessage = "The announcement can't be hidden before today.";
    private const string ActionMessage = "Choose where the button takes students.";
    private const string ActionUrlMessage = "Write a link that starts with https:// and has at most 500 characters.";

    public static Result<AnnouncementContent> From(
        string? title,
        string? body,
        AnnouncementCategory category,
        DateOnly? visibleUntil,
        DateOnly today,
        AnnouncementAction action,
        string? actionUrl)
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

        if (!Enum.IsDefined(action))
        {
            return Result.Validation<AnnouncementContent>(ActionMessage, fieldName: nameof(Announcement.Action));
        }

        var trimmedActionUrl = action == AnnouncementAction.Link ? actionUrl?.Trim() : null;
        if (action == AnnouncementAction.Link && !IsHttpsLink(trimmedActionUrl))
        {
            return Result.Validation<AnnouncementContent>(ActionUrlMessage, fieldName: nameof(Announcement.ActionUrl));
        }

        return new AnnouncementContent(trimmedTitle, trimmedBody, category, visibleUntil, action, trimmedActionUrl);
    }

    private static bool IsHttpsLink(string? link) =>
        link is { Length: > 0 and <= Announcement.ActionUrlMaxLength }
        && Uri.TryCreate(link, UriKind.Absolute, out var parsedLink)
        && parsedLink.Scheme == Uri.UriSchemeHttps;
}
