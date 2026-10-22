using ClassManager.Notifications.Email;

namespace ClassManager.Core.Emails;

public sealed record AppFeedbackEmailDetails(
    string Message,
    string SenderFullName,
    string SenderEmail,
    string BusinessName,
    string Role,
    string? Platform,
    string? AppVersion)
{
    private const char Separator = ' ';

    public IReadOnlyList<EmailLine> Lines(AppFeedbackEmailLabels labels)
    {
        var app = string.Join(Separator, new[] { Platform, AppVersion }.Where(part => !string.IsNullOrWhiteSpace(part)));
        IReadOnlyList<EmailLine> lines =
        [
            new(labels.Business, BusinessName),
            new(labels.Email, SenderEmail),
            new(labels.Role, Role),
        ];
        return app.Length == 0 ? lines : [.. lines, new(labels.App, app)];
    }
}

public sealed record AppFeedbackEmailLabels(string Business, string Email, string Role, string App);
