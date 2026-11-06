namespace ClassManager.Notifications.Email;

public sealed record OutgoingEmail(
    string To,
    string Subject,
    string? FromName,
    string TextBody,
    string HtmlBody,
    IReadOnlyList<EmailInlineImage> InlineImages,
    string? ReplyTo = null)
{
    public string? OneClickUnsubscribeUrl { get; init; }
}

public sealed record EmailInlineImage(string ContentId, byte[] Content, string ContentType);
