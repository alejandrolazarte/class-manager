using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Notifications;

public sealed class TeamNotification : ITenantOwned
{
    public const int TitleMaxLength = 120;
    public const int BodyMaxLength = 300;
    public const int UrlMaxLength = 200;

    private const string Ellipsis = "…";

    private TeamNotification()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? SeenAt { get; private set; }

    public static TeamNotification Create(Guid recipientUserId, string title, string body, string url, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            RecipientUserId = recipientUserId,
            Title = Shorten(title, TitleMaxLength),
            Body = Shorten(body, BodyMaxLength),
            Url = url[..Math.Min(url.Length, UrlMaxLength)],
            CreatedAt = createdAt.ToUniversalTime(),
        };

    public void MarkSeen(DateTimeOffset seenAt) => SeenAt ??= seenAt.ToUniversalTime();

    private static string Shorten(string text, int maxLength) =>
        text.Length <= maxLength ? text : string.Concat(text.AsSpan(0, maxLength - Ellipsis.Length), Ellipsis);
}
