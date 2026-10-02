namespace ClassManager.Infrastructure.Email;

public sealed record OutgoingEmail(
    string To,
    string Subject,
    string? FromName,
    string TextBody,
    string HtmlBody,
    IReadOnlyList<EmailInlineImage> InlineImages);

public sealed record EmailInlineImage(string ContentId, byte[] Content, string ContentType);
